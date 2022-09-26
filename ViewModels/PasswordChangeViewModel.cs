using System.ComponentModel.DataAnnotations;

namespace ChatApp.ViewModels;

public class PasswordChangeViewModel
{
    [Required]
    [Display(Name = "Old password")]
    public string OldPassword { get; set; }
    
    [Required]
    [MinLength(8, ErrorMessage = "Password is too short.")]
    [StringLength(32, ErrorMessage = "Password is too long.")]
    [Display(Name = "New password")]
    public string NewPassword { get; set; }
    
    [Required]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    [Display(Name = "Confirm new password")]
    public string ConfirmNewPassword { get; set; }
}