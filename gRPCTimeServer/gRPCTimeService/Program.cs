
using gRPCTimeService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace gRPCTimeService;

public class Program
{
    public static void Main(string[] args) => RunServer(args);

    private static void RunServer(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

        // Add services to the container.
        builder.Services.AddEntityFrameworkSqlite().AddDbContext<SqLiteContext>();
        builder.Services.AddGrpc();
        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = JwtConfig.Issuer,
                    IssuerSigningKey = JwtConfig.GetKey(),
                    ValidateAudience = false
                };
            });

        builder.Services.AddAuthorization();

        var app = builder.Build();
        using(var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SqLiteContext>();
            dbContext.Database.EnsureCreated();
        }

        // Configure the HTTP request pipeline.
        app.MapGrpcService<gRPCTimeService.Services.LoginService>();
        app.MapGrpcService<gRPCTimeService.Services.TimeService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.Run();
    }
}