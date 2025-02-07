using System;
using System.Threading.Tasks;
using GameScore;
using Grpc.Core;
using Grpc.Core.Logging;

namespace TestClient;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // Enable detailed logging
        GrpcEnvironment.SetLogger(new LogLevelFilterLogger(
            new ConsoleLogger(), LogLevel.Debug));

        // Create the channel with the correct host and port
        var channel = new Channel(
            "14.13.64.130", 56078,
            ChannelCredentials.Insecure,
            new[] {
                new ChannelOption(ChannelOptions.MaxSendMessageLength, 1024 * 1024 * 1024),
                new ChannelOption(ChannelOptions.MaxReceiveMessageLength, 1024 * 1024 * 1024),
                new ChannelOption(ChannelOptions.MaxConcurrentStreams, 1000),
            });

        // Show channel state changes
        channel.ShutdownToken.Register(() =>
        {
            Console.WriteLine("Channel shutting down...");
        });

        // Show channel state
        Console.WriteLine($"Channel state: {channel.State}");

        var client = new ScoreService.ScoreServiceClient(channel);
        var version = client.GetVersion(new Google.Protobuf.WellKnownTypes.Empty());

        Console.WriteLine($"gRPC server version: {version.Major}.{version.Minor}.{version.Patch}" + (version.Suffix.Length > 0 ? $"-{version.Suffix}" : ""));

        // Send scores using the old endpoint
        await SendByOldEndpointAsync(channel);

        // Send player game progress using the new endpoint
        //await SendByNewEndpointAsync(channel);

        await channel.ShutdownAsync();

    }

    private static async Task SendByOldEndpointAsync(Channel channel)
    {
        try
        {
            var service = new ScoreService.ScoreServiceClient(channel);
            var stream = service.StreamScores();

            // Send some scores
            var random = new Random();
            for (var i = 0; i < 100; i++)
            {
                var message = new ScoreRequest
                {
                    PlayerId = "player" + random.Next(1, 100),
                    Combo = random.Next(1, 10),
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    HitBombs = random.Next(0, 5),
                    MissedNotes = random.Next(0, 5),
                    PauseTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    RawScore = random.Next(50, 150),
                    SoftFailed = random.Next(0, 2) == 1,
                };

                try
                {
                    await stream.RequestStream.WriteAsync(message);
                }
                catch (RpcException ex)
                {
                    var isRecoverable = ex.StatusCode switch
                    {
                        StatusCode.Unavailable => true,
                        StatusCode.DeadlineExceeded => true,
                        StatusCode.ResourceExhausted => true,
                        _ => false
                    };

                    if (isRecoverable)
                    {
                        Console.WriteLine($"Recoverable error sending score: {ex.Status}");
                        await Task.Delay(300);
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending score: {ex.Message}");
                }

                await Task.Delay(500);
                Console.WriteLine($"Sent score {i + 1} of 100: {message}");
            }

            await stream.RequestStream.CompleteAsync();
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"gRPC error: {ex.Status}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }

    private static async Task SendByNewEndpointAsync(Channel channel)
    {
        var service = new ScoreService.ScoreServiceClient(channel);
        var stream = service.RealtimeScoreStream();

        // Send some player game progress
        var random = new Random();
        for (var i = 0; i < 100; i++)
        {
            try
            {
                await stream.RequestStream.WriteAsync(new PlayerGameProgress
                {
                    PlayerId = "player" + random.Next(1, 100),
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending player game progress: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(1));

            Console.WriteLine($"Sent player game progress {i + 1} of 100.");
        }

        await stream.RequestStream.CompleteAsync();
    }
}
