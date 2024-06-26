
namespace DataAccess
{
    public class MyDbConnection:IMyDbConnection
    {
        public string ConnectionString { get; set; }
        public MyDbConnection(string connectionString) 
        {
            ConnectionString = connectionString;
        }
    }
}
