using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using TimeService;

namespace gRPCTimeService.Services;

/// <summary>
/// Provides gRPC services for time-related operations.
/// </summary>
public class TimeService : Time.TimeBase
{
    /// <summary>
    /// Retrieves the current Coordinated Universal Time (UTC) based on the specified request.
    /// </summary>
    /// <param name="request">The gRPC request containing parameters for getting the current time.</param>
    /// <param name="context">The gRPC context for the server call.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> resulting in a <see cref="GetCurrentTimeUTCResponse"/>
    /// containing the current Coordinated Universal Time (UTC).
    /// </returns>
    public override async Task<GetCurrentTimeUTCResponse> GetCurrentTimeUTC(GetCurrentTimeUTCRequest request, ServerCallContext context)
    {
        var utcTimeNow = Timestamp.FromDateTime(DateTime.UtcNow);

        await using var db = new SqLiteContext();
        await db.TimestampEntries.AddAsync(new TimestampEntry(utcTimeNow.ToDateTime()));
        await db.SaveChangesAsync();
        
        return new GetCurrentTimeUTCResponse { UtcTime = utcTimeNow };
    }
    
    /// <summary>
    /// Gets time retrieval logs of all calls to <see cref="GetCurrentTimeUTC"/>.
    /// </summary>
    /// <param name="request">The gRPC request containing parameters for retrieving time retrieval logs.</param>
    /// <param name="context">The gRPC context for the server call.</param>
    /// <returns>/// A <see cref="Task{TResult}"/> resulting in a <see cref="GetTimeRetrievalLogsResponse"/>
    /// containing the retrieved time retrieval logs.</returns>
    [Authorize]
    public override async Task<GetTimeRetrievalLogsResponse> GetTimeRetrievalLogs(GetTimeRetrievalLogsRequest request, ServerCallContext context)
    {
        var response = new GetTimeRetrievalLogsResponse();
        
        await using var db = new SqLiteContext();
        response.TimeRetrievalLogs.AddRange(db.TimestampEntries.ToList().Select((v, i) => new TimeRetrievalLog { Id = i, UtcTime = v.Timestamp.ToTimestamp() }));

        return response;
    }
}