using Grpc.Core;
using LoginService;
using Microsoft.Extensions.DependencyInjection;
using TimeService;

namespace gRPCTimeService.IntegrationTests;

public class TimeServiceTests
{
    private readonly GRPCWebApplicationFactory<Program, Time.TimeClient> timeServiceFactory = new();
    private readonly GRPCWebApplicationFactory<Program, Login.LoginClient> loginServiceFactory = new();
    private readonly Metadata headers;
    private readonly Time.TimeClient client;

    public TimeServiceTests()
    {
        string? token = loginServiceFactory.RetrieveGrpcClient().CreateToken(new CreateTokenRequest()).Token;
        headers = new Metadata { { "Authorization", $"Bearer {token}" } };
        client = timeServiceFactory.RetrieveGrpcClient();
    }

    [SetUp]
    public void Setup()
    {
        var dbContext = timeServiceFactory.Services.GetService<SqLiteContext>()!;
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }

    [Test]
    public async Task GetCurrentTimeUtcTest()
    {
        var actualUtcTime = (await client.GetCurrentTimeUTCAsync(new GetCurrentTimeUTCRequest())).UtcTime.ToDateTime();
        var expectedUtcTime = DateTime.UtcNow;
        
        Assert.That(actualUtcTime, Is.EqualTo(expectedUtcTime).Within(TimeSpan.FromSeconds(2.0)));
    }
    
    [Test]
    public void GetTimeRetrievalLogs_WhenNotAuthenticated_ThenException()
    {
        Assert.That(
            () => client.GetTimeRetrievalLogs(new GetTimeRetrievalLogsRequest()),
            Throws.TypeOf<RpcException>().With.Message.Contains("Unauthenticated"));
    }
    
    [Test]
    public void GetTimeRetrievalLogs_WhenNoLogsExist_ReturnEmptyList()
    {
        List<TimeRetrievalLog> logs = client.GetTimeRetrievalLogs(new GetTimeRetrievalLogsRequest(), headers).TimeRetrievalLogs.ToList();

        Assert.That(logs, Is.Empty);
    }

    [Test]
    public void GetTimeRetrievalLogs_WhenLogsExist_ReturnLogs()
    {
        List<GetCurrentTimeUTCResponse> expectedLogs = Enumerable
            .Range(0, 5)
            .Select(_ => client.GetCurrentTimeUTC(new GetCurrentTimeUTCRequest()))
            .ToList();

        List<TimeRetrievalLog> actualLogs = client.GetTimeRetrievalLogs(new GetTimeRetrievalLogsRequest(), headers).TimeRetrievalLogs.ToList();
        
        Assert.That(actualLogs, Has.Count.EqualTo(expectedLogs.Count));

        for (var i = 0; i < actualLogs.Count; i++)
            Assert.That(actualLogs[i].UtcTime.ToDateTime(), Is.EqualTo(expectedLogs[i].UtcTime.ToDateTime()).Within(TimeSpan.FromSeconds(2.0)));
    }
}