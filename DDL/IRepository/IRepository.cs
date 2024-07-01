
namespace DAL.IRepository
{
    public interface IRepository<T> where T : class
    {
        int Add(string connectionString, T entity);
        IEnumerable<T>? GetAll(string connectionString, Dictionary<string, dynamic>? condition = null, string? includeProperties = null);
        T? Get(string connectionString, Dictionary<string, dynamic> condition, string? includeProperties = null);
        T? Update(string connectionString, T entity);
        int Remove(string connectionString, string TableName, string ColumnName, int Id);
        int RemoveRange(string connectionString, string TableName, string ColumnName, List<int> Ids);
    }
}
