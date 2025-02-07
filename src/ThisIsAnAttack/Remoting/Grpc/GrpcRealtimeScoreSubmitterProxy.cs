using System.Runtime.CompilerServices;
using ThisIsAnAttack.Logging;
using ThisIsAnAttack.Monitors.Scoring.Entities;
using Zenject;

namespace ThisIsAnAttack.Remoting.Grpc;

public class GrpcRealtimeScoreSubmitterProxy : IRealtimeScoreSubmitter, IDisposable
{
    public IPluginLogger Logger { get; }
    public DiContainer Container { get; }

    private volatile GrpcRealtimeScoreSubmitter? submitter_;

    public GrpcRealtimeScoreSubmitterProxy(
        IPluginLogger logger,
        DiContainer container)
    {
        this.Logger = logger;
        this.Container = container;
    }

    /// <summary>
    /// Creates a new <see cref="GrpcRealtimeScoreSubmitter"/> instance if not created yet.
    /// If the instance is already created but the stream is not available, it will try to renew the stream, by recreating the submitter.
    /// </summary>
    /// <returns></returns>
    private GrpcRealtimeScoreSubmitter RenewSubmitterIfNecessary()
    {
        if (this.submitter_ is { IsStreamAlive: true } currentSubmitter)
        {
            return currentSubmitter;
        }

        if (this.submitter_ is { IsStreamAlive: false } oldSubmitter)
        {
            oldSubmitter.Dispose();
        }

        return this.submitter_ = this.Container.Instantiate<GrpcRealtimeScoreSubmitter>();
    }

    public async Task SubmitFinishScoreAsync(PlayerGameProgress progress)
    {
        var submitter = this
            .RenewSubmitterIfNecessary();

        await submitter
            .SubmitFinishScoreAsync(progress)
            .ConfigureAwait(false);
    }

    public async Task SubmitScoreAsync(PlayerGameProgress score)
    {
        var submitter = this
            .RenewSubmitterIfNecessary();

        await submitter
            .SubmitScoreAsync(score)
            .ConfigureAwait(false);
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
                this.submitter_?.Dispose();
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
            throw new ObjectDisposedException(this.GetObjectName());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual string GetObjectName()
    {
        return this.ToString() ?? this.GetType().Name;
    }

    #endregion IDisposable
}
