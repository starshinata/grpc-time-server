
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace gRPCTimeService.Services;

/// <summary>
/// Stores configuration related to JWT (JSON Web Token).
///</summary>
public static class JwtConfig
{
    public const string Issuer = "me";
    private const string Secret = "veryStrongSecurityKey123";

    public static SymmetricSecurityKey GetKey() => new(Encoding.UTF8.GetBytes(Secret));
}