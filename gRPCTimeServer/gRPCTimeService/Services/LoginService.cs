using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Grpc.Core;
using LoginService;
using Microsoft.IdentityModel.Tokens;

namespace gRPCTimeService.Services;

public class LoginService : Login.LoginBase
{
    public override Task<CreateTokenResponse> CreateToken(CreateTokenRequest request, ServerCallContext context)
    {
        return Task.FromResult(new CreateTokenResponse { Token = GenerateToken() });
    }
    
    private static string GenerateToken()
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Name", "Alcatraz"),
            }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            Issuer = JwtConfig.Issuer,
            SigningCredentials = new SigningCredentials(JwtConfig.GetKey(), SecurityAlgorithms.HmacSha512Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}