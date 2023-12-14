using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Grpc.Core;
using LoginService;
using Microsoft.IdentityModel.Tokens;

namespace gRPCTimeService.Services;

/// <summary>
/// Provides gRPC services for login-related operations.
/// </summary>
public class LoginService : Login.LoginBase
{
    /// <summary>
    /// Creates a JWT token based on the specified request.
    /// </summary>
    /// <param name="request">The gRPC request containing parameters for getting the token.</param>
    /// <param name="context">The gRPC context for the server call.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> resulting in a <see cref="CreateTokenResponse"/>
    /// containing the newly-generated token.
    /// </returns>
    public override Task<CreateTokenResponse> CreateToken(CreateTokenRequest request, ServerCallContext context)
    {
        // This is a PoC. In a real-life scenario, some login details would need to be validated before creating a token.
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