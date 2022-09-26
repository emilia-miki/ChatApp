using Microsoft.IdentityModel.Tokens;

namespace ChatApp.BLL;

public interface IAuthOptions
{
    string Issuer { get; }
    string Audience { get; }
    SymmetricSecurityKey Key { get; }
    int Lifetime { get; }
}