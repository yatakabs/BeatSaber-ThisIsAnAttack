using GameScore;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace MockGrpcService.Services;
public class ScoreService : GameScore.ScoreService.ScoreServiceBase
{
    private ILogger<ScoreService> Logger { get; }
    public ScoreService(ILogger<ScoreService> logger)
    {
        this.Logger = logger;

        this.Logger.LogInformation("ScoreService instance created");
    }

    public override async Task<Empty> StreamScores(IAsyncStreamReader<ScoreRequest> requestStream, ServerCallContext context)
    {
        using var scope = this.Logger.BeginScope(new
        {
            context.RequestHeaders,
            context.Host,
            context.Method,
            context.Peer,
            context.Status.StatusCode,
            context.Status.Detail,
            context.Status.DebugException,
            context.Status,
            context.ResponseTrailers,
        });

        this.Logger.LogInformation("Received request");

        while (await requestStream.MoveNext())
        {
            var scoreRequest = requestStream.Current;
            this.Logger.LogInformation("Received score: {@ScoreRequest}", scoreRequest);
        }

        return new Empty();
    }

    public override async Task<Empty> RealtimeScoreStream(IAsyncStreamReader<PlayerGameProgress> requestStream, ServerCallContext context)
    {
        using var scope = this.Logger.BeginScope(new
        {
            context.RequestHeaders,
            context.Host,
            context.Method,
            context.Peer,
            context.Status.StatusCode,
            context.Status.Detail,
            context.Status.DebugException,
            context.Status,
            context.ResponseTrailers,
        });

        this.Logger.LogInformation("Received request");

        try
        {
            while (await requestStream.MoveNext())
            {
                var playerGameProgress = requestStream.Current;
                this.Logger.LogInformation("Received player game progress: {@PlayerGameProgress}", playerGameProgress);
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "An error occurred while processing the request");
        }

        return new Empty();
    }
}
