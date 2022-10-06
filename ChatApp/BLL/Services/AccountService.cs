using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ChatApp.BLL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ChatApp.BLL.Models;
using ChatApp.DAL.Entities;
using ChatApp.ViewModels;

namespace ChatApp.BLL.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAuthOptions _authOptions;
    
    public AccountService(
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager,
        IAuthOptions authOptions)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authOptions = authOptions;
    }

    public async Task<bool> IsValidUserCredentialsAsync(LoginUser model)
    {
        var user = await _userManager.FindByNameAsync(model.Login);
        if (user == null)
        {
            return false;
        }
        
        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user, model.Password, false);
        return signInResult.Succeeded;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Login,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        return result;
    }

    public async Task<LoginResult> LoginAsync(LoginUser model)
    {
        var user = await _userManager.FindByNameAsync(model.Login);
        var roles = await _userManager.GetRolesAsync(user);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, model.Login)
        };

        if (roles != null)
        {
            claims.AddRange(roles.Select(role => 
                new Claim(ClaimTypes.Role, role)));
        }
        
        var now = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(
            issuer: _authOptions.Issuer,
            audience: _authOptions.Audience,
            notBefore: now,
            claims: claims,
            expires: now.AddDays(_authOptions.Lifetime),
            signingCredentials: new SigningCredentials(
                _authOptions.Key, SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new LoginResult(encodedJwt, model.Login);
    }

    public async Task<IdentityResult> ChangePasswordAsync(
        string login, string newPassword)
    {
        var user = 
            await _userManager.FindByNameAsync(login);
        var token = 
            await _userManager.GeneratePasswordResetTokenAsync(user);
        if (token == null)
        {
            return new IdentityResult();
        }
        
        return await _userManager.ResetPasswordAsync(user, token, newPassword);
    }

    public async Task DeleteAsync(string login)
    {
        var user = await _userManager.FindByNameAsync(login);
        await _userManager.DeleteAsync(user);
    }
}