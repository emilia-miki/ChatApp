using ChatApp.BLL.Models;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.BLL;

public interface IAccountService
{
    Task<bool> IsValidUserCredentials(LoginUser model);
    Task<IdentityResult> Register(RegisterViewModel model);
    Task<LoginResult> Login(LoginUser model);
    Task<IdentityResult> ChangePassword(
        HttpContext context, string newPassword);
    Task Delete(HttpContext context);
}