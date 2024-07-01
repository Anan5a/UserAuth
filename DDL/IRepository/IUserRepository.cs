using BE;

namespace DAL.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        new int Add(string connectionString, User user);
        new IEnumerable<User> GetAll(string connectionString, Dictionary<string, dynamic>? condition, string? includeProperties = null);
        new User? Get(string connectionString, Dictionary<string, dynamic> condition, string? includeProperties);
        new User? Update(string connectionString, User user);
        int Remove(string connectionString, int id);
        int RemoveRange(string connectionString, List<int> Ids);
    }
}
