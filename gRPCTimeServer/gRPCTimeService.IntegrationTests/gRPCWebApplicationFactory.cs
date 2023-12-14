using Grpc.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace gRPCTimeService.IntegrationTests;

public class GRPCWebApplicationFactory<TStartup, TClient> : WebApplicationFactory<TStartup>
    where TStartup : class
    where TClient : ClientBase<TClient>
{
    #region Overrides of WebApplicationFactory<T>

    /// <summary>
    /// Gives a fixture an opportunity to configure the application before it gets built.
    /// </summary>
    /// <param name="builder">The <see cref="T:Microsoft.AspNetCore.Hosting.IWebHostBuilder" /> for the application.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder
            .UseEnvironment("Test")
            .ConfigureServices(services =>
            {
                services
                    .AddEntityFrameworkSqlite().AddDbContext<SqLiteContext>()
                    .AddGrpcClient<TClient>(options => options.Address = new Uri("http://localhost"))
                    .ConfigurePrimaryHttpMessageHandler(() => Server.CreateHandler());
            });

    #endregion

    public TClient RetrieveGrpcClient()
    {
        return Services.GetRequiredService<TClient>();
    }
}