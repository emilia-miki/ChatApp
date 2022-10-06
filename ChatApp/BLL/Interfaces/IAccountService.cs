using ChatApp.BLL.Models;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.BLL.Interfaces;

public interface IAccountService
{
    Task<bool> IsValidUserCredentialsAsync(LoginUser model);
    Task<IdentityResult> RegisterAsync(RegisterViewModel model);
    Task<LoginResult> LoginAsync(LoginUser model);
    Task<IdentityResult> ChangePasswordAsync(
        string login, string newPassword);
    Task DeleteAsync(string login);
}