using BE;

namespace DAL.IRepository
{
    public interface IPasswordResetRepository : IRepository<PasswordResetEntity>
    {
        new int Add(string connectionString, PasswordResetEntity user);
        new void GetAll(string connectionString, Dictionary<string, dynamic>? condition, string? includeProperties = null);
        new PasswordResetEntity? Get(string connectionString, Dictionary<string, dynamic> condition, string? includeProperties);
        new void Update(string connectionString);
        int Remove(string connectionString, int id);
        int RemoveRange(string connectionString, List<int> Ids);
    }
}
