
namespace IRepository
{
    public interface IRepository<T> where T : class
    {
        IMyDbConnection dbConnection { get; set; }
        int Add(T entity);
        IEnumerable<T>? GetAll(Dictionary<string, dynamic>? condition = null, string? includeProperties = null);
        T? Get(Dictionary<string, dynamic> condition, string? includeProperties = null);
        T? Update(T entity);
        int Remove(string TableName, string ColumnName, int Id);
        int RemoveRange(string TableName, string ColumnName, List<int> Ids);
    }
}
