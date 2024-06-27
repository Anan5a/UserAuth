
namespace DataAccess
{
    public class MyDbConnection
    {
        public string ConnectionString { get; set; }
        public MyDbConnection(string connectionString) 
        {
            ConnectionString = connectionString;
        }
    }
}
