
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace BE.DTOs
{
    public class ChangePasswordDto
    {
        public long? Id { get; set; }
        
        public string? Name { get; set; }


        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [Display(Prompt ="Enter your password")]
        public string Password { get; set; }

        [StringLength(50, MinimumLength = 6, ErrorMessage = "New Password must be at least 6 characters long")]
        [Display(Prompt = "Enter new password")]
        [DisplayName("New Password")]
        public string? NewPassword { get; set; }

        [StringLength(50, MinimumLength = 6, ErrorMessage = "Confirm New Password must be at least 6 characters long")]
        [Compare("NewPassword", ErrorMessage="New password does not match")]
        [Display(Prompt = "Enter new password")]
        [DisplayName("Confirm New Password")]
        public string? ConfirmNewPassword { get; set; }
    }
}
