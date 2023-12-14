using Microsoft.Extensions.DependencyInjection;
using TimeService;

namespace gRPCTimeService.IntegrationTests;

public class TimeServiceTests
{
    private readonly GRPCWebApplicationFactory<Program, Time.TimeClient> factory = new();

    [SetUp]
    public void Setup()
    {
        var dbContext = factory.Services.GetService<SqLiteContext>()!;
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }

    [Test]
    public async Task GetCurrentTimeUtcTest()
    {
        var client = factory.RetrieveGrpcClient();
        var actualUtcTime = (await client.GetCurrentTimeUTCAsync(new GetCurrentTimeUTCRequest())).UtcTime.ToDateTime();
        var expectedUtcTime = DateTime.UtcNow;
        
        Assert.That(actualUtcTime, Is.EqualTo(expectedUtcTime).Within(TimeSpan.FromSeconds(2.0)));
    }

    [Test]
    public void GetTimeRetrievalLogs_WhenNoLogsExist_ReturnEmptyList()
    {
        var client = factory.RetrieveGrpcClient();
        
        List<TimeRetrievalLog> logs = client.GetTimeRetrievalLogs(new GetTimeRetrievalLogsRequest()).TimeRetrievalLogs.ToList();

        Assert.That(logs, Is.Empty);
    }

    [Test]
    public void GetTimeRetrievalLogs_WhenLogsExist_ReturnLogs()
    {
        var client = factory.RetrieveGrpcClient();

        List<GetCurrentTimeUTCResponse> expectedLogs = Enumerable
            .Range(0, 5)
            .Select(_ => client.GetCurrentTimeUTC(new GetCurrentTimeUTCRequest()))
            .ToList();

        List<TimeRetrievalLog> actualLogs = client.GetTimeRetrievalLogs(new GetTimeRetrievalLogsRequest()).TimeRetrievalLogs.ToList();
        
        Assert.That(actualLogs, Has.Count.EqualTo(expectedLogs.Count));

        for (var i = 0; i < actualLogs.Count; i++)
            Assert.That(actualLogs[i].UtcTime.ToDateTime(), Is.EqualTo(expectedLogs[i].UtcTime.ToDateTime()).Within(TimeSpan.FromSeconds(2.0)));
    }
}