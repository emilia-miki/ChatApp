using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ChatApp.BLL.Models;
using ChatApp.ViewModels;

namespace ChatApp.BLL;

public class AccountService : IAccountService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IAuthOptions _authOptions;
    
    public AccountService(
        UserManager<IdentityUser> userManager, 
        SignInManager<IdentityUser> signInManager,
        IAuthOptions authOptions)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authOptions = authOptions;
    }

    public async Task<bool> IsValidUserCredentials(LoginUser model)
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

    public async Task<IdentityResult> Register(RegisterViewModel model)
    {
        var user = new IdentityUser {UserName = model.Login};
        var result = await _userManager.CreateAsync(user, model.Password);
        return result;
    }

    public async Task<LoginResult> Login(LoginUser model)
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

    public async Task<IdentityResult> ChangePassword(
        HttpContext context, string newPassword)
    {
        var user = 
            await _userManager.FindByNameAsync(context.User.Identity!.Name);
        var token = 
            await _userManager.GeneratePasswordResetTokenAsync(user);
        if (token == null)
        {
            return new IdentityResult();
        }
        
        return await _userManager.ResetPasswordAsync(user, token, newPassword);
    }
}