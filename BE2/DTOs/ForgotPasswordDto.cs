
using System.ComponentModel.DataAnnotations;


namespace BE.DTOs
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Prompt ="Enter your email address.")]
        public string Email { get; set; }
    }
}
