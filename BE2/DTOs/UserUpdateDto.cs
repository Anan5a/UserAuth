
using System.ComponentModel.DataAnnotations;


namespace BE.DTOs
{
    public class UserUpdateDto
    {
        public long? Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MinLength(4)]
        [MaxLength(100)]
        [Display(Prompt ="Enter your full name.")]

        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Prompt ="Enter your email address.")]
        public string Email { get; set; }

        
    }
}
