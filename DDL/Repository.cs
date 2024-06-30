using IRepository;
using Microsoft.Data.SqlClient;


namespace DataAccess
{
    public class Repository<T> : IRepository<T> where T : class
    {
        
        
        public int Remove(string connectionString, string TableName, string ColumnName, int Id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = $"DELETE FROM {TableName} WHERE {ColumnName} = @Id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected;
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public int RemoveRange(string connectionString, string TableName, string ColumnName, List<int> Ids)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = $"DELETE FROM {TableName} WHERE {ColumnName} IN ({string.Join(",", Ids)})";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected;
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int Add(string connectionString, T entity)
        {
            throw new NotImplementedException("Must implement specific logic in the repository");
        }

        public IEnumerable<T> GetAll(string connectionString, Dictionary<string, dynamic>? condition = null, string ? includeProperties = null)
        {
            throw new NotImplementedException("Must implement specific logic in the repository");
        }

        public T? Get(string connectionString, Dictionary<string, dynamic> condition, string? includeProperties)
        {
            throw new NotImplementedException("Must implement specific logic in the repository");
        }

        public T Update(string connectionString, T entity)
        {
            throw new NotImplementedException("Must implement specific logic in the repository");
        }
    }
}
