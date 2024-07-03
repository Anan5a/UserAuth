
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace BE.DTOs
{
    public class ResetPasswordDto
    {
        public string? Token { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "New Password must be at least 6 characters long")]
        [Display(Prompt = "Enter new password")]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Confirm New Password must be at least 6 characters long")]
        [Compare("NewPassword", ErrorMessage = "New password does not match")]
        [Display(Prompt = "Enter new password")]
        [DisplayName("Confirm New Password")]
        public string ConfirmNewPassword { get; set; }
    }
}
