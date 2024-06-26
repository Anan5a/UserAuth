using System.ComponentModel.DataAnnotations;


namespace Models.DTOs
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Prompt ="Enter your email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [Display(Prompt ="Enter your password.")]
        public string Password { get; set; }
    }
}
