
using System.ComponentModel.DataAnnotations;


namespace Models.DTOs
{
    public class UserSignupDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(4)]
        [MaxLength(100)]
        [Display(Prompt ="Enter your full name.")]

        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Prompt ="Enter your email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [Display(Prompt ="Enter new password")]
        public string Password { get; set; }
    }
}
