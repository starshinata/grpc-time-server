// See https://aka.ms/new-console-template for more information

using Google.Protobuf.Collections;
using Grpc.Core;
using Grpc.Net.Client;
using LoginService;
using TimeService;

using var channel = GrpcChannel.ForAddress("http://localhost:5000");
var timeServiceClient = new Time.TimeClient(channel);
var loginServiceClient = new Login.LoginClient(channel);

var token = loginServiceClient.CreateToken(new CreateTokenRequest()).Token;
var headers = new Metadata { { "Authorization", $"Bearer {token}" } };

RunCli(timeServiceClient);

return;

// ReSharper disable once InvalidXmlDocComment
/// <summary>
/// Runs the command-line interface to interact with a gRPC time service.
/// </summary>
/// <param name="timeClient">The gRPC client for the time service.</param>
void RunCli(Time.TimeClient timeClient)
{
    Console.WriteLine("Press '1' to call GetCurrentTimeUTC. Press '2' to call GetTimeRetrievalLogs. Press 'q' to quit.");

    while (true)
    {
        var keyInfo = Console.ReadKey();
        Console.WriteLine();

        try
        {
            // Check the pressed key and call the appropriate method.
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (keyInfo.Key)
            {
                case ConsoleKey.D1:
                    var utcTime = timeClient.GetCurrentTimeUTC(new GetCurrentTimeUTCRequest()).UtcTime;
                    Console.WriteLine("Got timestamp from server: " + utcTime);
                    break;
                case ConsoleKey.D2:
                    Console.WriteLine("Got logs from server:\n");
                    RepeatedField<TimeRetrievalLog>? logs =
                        timeClient.GetTimeRetrievalLogs(new GetTimeRetrievalLogsRequest(), headers:headers).TimeRetrievalLogs;
                    logs.ToList().ForEach(l => Console.WriteLine(l.UtcTime.ToString()));
                    break;
                case ConsoleKey.Q:
                    Console.WriteLine("\nExiting the program.");
                    return;
                default:
                    Console.WriteLine("\nInvalid key. Press 1, 2, or 'q'.");
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}