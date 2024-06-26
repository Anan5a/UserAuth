using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models
{
    public class User
    {
        [BindNever]
        [ValidateNever]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [JsonIgnore]
        public string Password { get; set; }
        [ValidateNever]
        [BindNever]
        public DateTime CreatedAt { get; set; }
        [BindNever]
        [ValidateNever]
        public DateTime? ModifiedAt { get; set; }
        
    }
}
