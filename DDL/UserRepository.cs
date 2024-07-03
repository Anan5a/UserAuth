using Microsoft.Data.SqlClient;
using BE;
using System.Data;
using System.Text;
using DAL.IRepository;
using Newtonsoft.Json;

namespace DAL
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly string tableName = "Users";
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
        public new int Add(string connectionString, User user)
        {
            Logger.Debug("Starting UserRepository::Add with param:{0}", JsonConvert.SerializeObject(user));

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = $"INSERT INTO [{tableName}] ([Name], [Email], [Password], [CreatedAt], [ModifiedAt]) VALUES (@Name, @Email, @Password, @CreatedAt, @ModifiedAt); SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Name", user.Name);
                        command.Parameters.AddWithValue("@Email", user.Email);
                        command.Parameters.AddWithValue("@Password", user.Password);
                        command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
                        command.Parameters.AddWithValue("@ModifiedAt", user.ModifiedAt);
                        int insertedUserId = Convert.ToInt32(command.ExecuteScalar());
                        Logger.Debug("Added user, entry id:{0}", insertedUserId);
                        Logger.Info("End UserRepository::Add");
                        return insertedUserId;
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Info("Add user failed");

                Logger.Info("End UserRepository::Add");
                return 0;
            }



        }

        public new IEnumerable<User> GetAll(string connectionString, Dictionary<string, dynamic>? condition = null, string? includeProperties = "true")
        {
            Logger.Debug("Starting UserRepository::GetAll with param:{0}", JsonConvert.SerializeObject(condition?.ToArray()));

            List<User> users = new List<User>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                StringBuilder whereClause = new StringBuilder();
                List<SqlParameter> parameters = new List<SqlParameter>();
                if (condition != null)
                {
                    foreach (var pair in condition)
                    {
                        if (whereClause.Length > 0)
                            whereClause.Append(" AND ");

                        whereClause.Append($"u.[{pair.Key}] = @{pair.Key}");
                        parameters.Add(new SqlParameter($"@{pair.Key}", pair.Value));
                    }

                }


                string query;
                if (whereClause.Length > 0)
                {
                    query = $"SELECT u.[Id], u.[Name], u.[Email], u.[Password], u.[CreatedAt], u.[ModifiedAt] FROM [{tableName}] u WHERE {whereClause}";
                }
                else
                {
                    query = $"SELECT u.[Id], u.[Name], u.[Email], u.[Password], u.[CreatedAt], u.[ModifiedAt] FROM [{tableName}] u";
                }


                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        foreach (DataRow row in dataTable.Rows)
                        {
                            User user = new User
                            {
                                Id = Convert.ToInt64(row["Id"]),
                                Name = Convert.ToString(row["Name"]),
                                Email = Convert.ToString(row["Email"]),
                                Password = Convert.ToString(row["Password"]),
                                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                                ModifiedAt = Convert.ToDateTime(row["ModifiedAt"])
                            };

                            users.Add(user);
                        }
                    }
                }
            }

            Logger.Info("End UserRepository::GetAll");

            return users;
        }

        public new User? Get(string connectionString, Dictionary<string, dynamic> condition, string? includeProperties)
        {
            Logger.Debug("Starting UserRepository::Get with param:{0}", JsonConvert.SerializeObject(condition?.ToArray()));

            User? user = null;
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
                    query = $"SELECT u.[Id], u.[Name], u.[Email], u.[Password], u.[CreatedAt], u.[ModifiedAt] FROM [{tableName}] u WHERE {whereClause}";
                }
                else
                {
                    query = $"SELECT u.[Id], u.[Name], u.[Email], u.[Password], u.[CreatedAt], u.[ModifiedAt] FROM [{tableName}] u WHERE {whereClause}";
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                Id = Convert.ToInt64(reader["Id"]),
                                Name = Convert.ToString(reader["Name"]),
                                Email = Convert.ToString(reader["Email"]),
                                Password = Convert.ToString(reader["Password"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                ModifiedAt = Convert.ToDateTime(reader["ModifiedAt"])
                            };

                        }
                    }
                }
            }
            Logger.Info("End UserRepository::Get");

            return user;
        }

        public new User? Update(string connectionString, User existingUser)
        {
            Logger.Debug("Starting UserRepository::Update with param:{0}", JsonConvert.SerializeObject(existingUser));

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Execute the SQL update command to update the user record in the database
                string query = $"UPDATE [{tableName}] SET ";

                if (existingUser.Name != null)
                {
                    query += "[Name] = @Name,";
                }
                if (existingUser.Email != null)
                {
                    query += "[Email] = @Email,";
                }
                if (existingUser.Password != null)
                {
                    query += "[Password] = @Password,";
                }
                query += "[ModifiedAt] = @ModifiedAt WHERE [Id] = @Id";


                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (existingUser.Name!=null)
                    {
                        command.Parameters.AddWithValue("@Name", existingUser.Name);
                    }
                    if (existingUser.Password != null)
                    {
                        command.Parameters.AddWithValue("@Password", existingUser.Password);

                    }
                    if (existingUser.Email!=null)
                    {
                        command.Parameters.AddWithValue("@Email", existingUser.Email);

                    }
                    command.Parameters.AddWithValue("@ModifiedAt", existingUser.ModifiedAt);
                    command.Parameters.AddWithValue("@Id", existingUser.Id);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        Logger.Info("No data changed");
                        Logger.Info("End UserRepository::Update");

                        // If no rows were affected, the update operation failed
                        return null;
                    }
                }
            }
            Logger.Info("End UserRepository::Update");

            return existingUser;
        }
    }
}
