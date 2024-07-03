using Microsoft.Data.SqlClient;
using BE;
using System.Data;
using System.Text;
using DAL.IRepository;
using Newtonsoft.Json;

namespace DAL
{
    public class PasswordResetRepository : Repository<PasswordResetEntity>, IPasswordResetRepository
    {
        private readonly string tableName = "PasswordReset";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        public int Remove(string connectionString, int Id)
        {
            Logger.Debug("Removing data with id:{0}", Id);
            return base.Remove(connectionString, tableName, "Id", Id);
        }
        public int RemoveRange(string connectionString, List<int> Ids)
        {
            Logger.Debug("Removing data with ids:{0}", Ids.ToString());

            return base.RemoveRange(connectionString, tableName, "Id", Ids);
        }
        public new int Add(string connectionString, PasswordResetEntity passwordResetEntity)
        {
            Logger.Debug("Starting PasswordResetRepository::Add with param:{0}", JsonConvert.SerializeObject(passwordResetEntity));

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = $"INSERT INTO [{tableName}] ([Token], [UserId], [ExpiresAt]) VALUES (@Token, @UserId, @ExpiresAt); SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Token", passwordResetEntity.Token);
                        command.Parameters.AddWithValue("@UserId", passwordResetEntity.UserId);
                        command.Parameters.AddWithValue("@ExpiresAt", passwordResetEntity.ExpiresAt);
                        int insertedUserId = Convert.ToInt32(command.ExecuteScalar());
                        Logger.Debug("Added password reset token, entry id:{0}", insertedUserId);
                        Logger.Info("End PasswordResetRepository::Add");
                        return insertedUserId;
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Info("Add password reset token failed");

                Logger.Info("End PasswordResetRepository::Add");
                return 0;
            }



        }

        public new void GetAll(string connectionString, Dictionary<string, dynamic>? condition = null, string? includeProperties = "true")
        {
            Logger.Debug("Starting PasswordResetRepository::GetAll with param:{0}", JsonConvert.SerializeObject(condition?.ToArray()));
            Logger.Info("Invalid operation, cannot get all tokens!");
            
            Logger.Info("End PasswordResetRepository::GetAll");

        }

        public new PasswordResetEntity? Get(string connectionString, Dictionary<string, dynamic> condition, string? includeProperties)
        {
            Logger.Debug("Starting PasswordResetRepository::Get with param:{0}", JsonConvert.SerializeObject(condition?.ToArray()));

            PasswordResetEntity? user = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Build the WHERE clause based on the conditions in the dictionary
                StringBuilder whereClause = new StringBuilder();
                List<SqlParameter> parameters = new List<SqlParameter>();
                foreach (var pair in condition)
                {
                    if (whereClause.Length > 0)
                        whereClause.Append(" AND ");

                    whereClause.Append($"u.[{pair.Key}] = @{pair.Key}");
                    parameters.Add(new SqlParameter($"@{pair.Key}", pair.Value));
                }
                string query;

                if (includeProperties != null)
                {
                    query = $"SELECT u.[Id], u.[UserId], u.[Token], u.[ExpiresAt] FROM [{tableName}] u WHERE {whereClause}";
                }
                else
                {
                    query = $"SELECT u.[Id], u.[UserId], u.[Token], u.[ExpiresAt] FROM [{tableName}] u WHERE {whereClause}";
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new PasswordResetEntity
                            {
                                Id = Convert.ToInt64(reader["Id"]),
                                UserId = Convert.ToInt64(reader["UserId"]),
                                Token = Convert.ToString(reader["Token"]),
                                ExpiresAt = Convert.ToDateTime(reader["ExpiresAt"])
                            };

                        }
                    }
                }
            }
            Logger.Info("End PasswordResetRepository::Get");

            return user;
        }

        public void Update(string connectionString)
        {
            Logger.Debug("Starting PasswordResetRepository::Update");
            Logger.Info("Invalid operation, cannot update tokens!");

            Logger.Info("End PasswordResetRepository::Update");

        }
    }
}
