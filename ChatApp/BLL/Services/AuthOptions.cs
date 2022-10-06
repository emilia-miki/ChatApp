using System.Text;
using ChatApp.BLL.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace ChatApp.BLL.Services;

public class AuthOptions : IAuthOptions
{
    private readonly IConfigurationSection _jwt;

    public string Issuer => _jwt["Issuer"];
    public string Audience => _jwt["Audience"];
    public SymmetricSecurityKey Key => new(Encoding.UTF8.GetBytes(_jwt["Key"]));
    public int Lifetime => int.Parse(_jwt["Lifetime"]);

    public AuthOptions(IConfiguration configuration)
    {
        _jwt = configuration.GetSection("Jwt");
    }
}