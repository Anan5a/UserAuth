
namespace UserAuth.Models
{
    public class AccountViewModel
    {
        public required long Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime? ModifiedAt { get; set; }
    }
}
