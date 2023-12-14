
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace gRPCTimeService.Services;

public static class JwtConfig
{
    public const string Issuer = "me";
    public const string Secret = "veryStrongSecurityKey123";

    public static SymmetricSecurityKey GetKey() => new(Encoding.UTF8.GetBytes(Secret));
}