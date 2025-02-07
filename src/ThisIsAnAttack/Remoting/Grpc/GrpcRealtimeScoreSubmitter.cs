using System.Runtime.CompilerServices;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ThisIsAnAttack.Logging;
using ThisIsAnAttack.Monitors.Scoring.Entities;
using ScoreService = GameScore.ScoreService;

namespace ThisIsAnAttack.Remoting.Grpc;

public class GrpcRealtimeScoreSubmitter : IRealtimeScoreSubmitter, IDisposable
{
    private IPluginLogger Logger { get; }
    private Channel Channel { get; }
    private ScoreService.ScoreServiceClient Client { get; }
    private AsyncClientStreamingCall<GameScore.PlayerGameProgress, Empty> RealtimeScoreStream { get; }

    public bool IsStreamAlive { get; private set; }

    public GrpcRealtimeScoreSubmitter(
        Channel channel,
        IPluginLogger logger)
    {
        this.Channel = channel;
        this.Logger = logger;

        var client = new ScoreService.ScoreServiceClient(channel);
        this.Client = client;

        this.RealtimeScoreStream = client.RealtimeScoreStream();
        this.IsStreamAlive = true;

        Task.Run(async () =>
        {
            try
            {
                await this.RealtimeScoreStream
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Failed to establish the realtime score stream.");
                this.IsStreamAlive = false;
            }
        });
    }

    public async Task SubmitScoreAsync(PlayerGameProgress score)
    {
        this.ThrowIfDisposed();

        this.Logger.Debug($"Sending the score to the server. (PlayerId: {score.PlayerId})");

        try
        {
            // Send the score using the new endpoint: RealtimeScoreStream
            var message = score.ToGrpcSchema();

            await this.RealtimeScoreStream
                .RequestStream
                .WriteAsync(message)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex, "Failed to send the score to the server.");
        }
    }

    public async Task SubmitFinishScoreAsync(PlayerGameProgress progress)
    {
        this.ThrowIfDisposed();

        await this.SubmitScoreAsync(progress)
            .ConfigureAwait(false);

        this.Logger.Debug("Finish score submitted.");

        try
        {
            this.Logger.Debug($"Completing the score submission.");

            await this.RealtimeScoreStream
                .RequestStream
                .CompleteAsync()
                .ConfigureAwait(false);

            this.Logger.Debug($"Score submission completed.");
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex, "Failed to complete the score submission.");
        }
    }

    #region IDisposable

    private volatile bool isDisposed;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void Dispose(bool disposing)
    {
        if (!this.isDisposed)
        {
            if (disposing)
            {
                this.RealtimeScoreStream.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            this.isDisposed = true;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfDisposed()
    {
        if (this.isDisposed)
        {
            throw new ObjectDisposedException(nameof(GrpcRealtimeScoreSubmitter));
        }
    }

    #endregion IDisposable
}
