using System.ComponentModel.DataAnnotations;

namespace ChatApp.ViewModels;

public class RegisterViewModel
{
    [Required]
    [MinLength(1, ErrorMessage = "Login is empty.")]
    [StringLength(32, ErrorMessage = "Login is too long.")]
    public string Login { get; set; }
    
    [Required]
    [MinLength(8, ErrorMessage = "Password is too short.")]
    [StringLength(32, ErrorMessage = "Password is too long.")]
    public string Password { get; set; }
    
    [Required]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string PasswordConfirmation { get; set; }
}