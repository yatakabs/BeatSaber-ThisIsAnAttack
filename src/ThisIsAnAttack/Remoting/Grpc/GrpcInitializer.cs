using System.Runtime.CompilerServices;
using Grpc.Core;
using SiraUtil.Zenject;
using ThisIsAnAttack.Logging;

namespace ThisIsAnAttack.Remoting.Grpc;

/// <summary>
/// Initializes the gRPC connection asynchronously.
/// </summary>
public class GrpcInitializer : IAsyncInitializable, IDisposable
{
    private IPluginLogger Logger { get; }
    private Channel Channel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrpcInitializer"/> class.
    /// </summary>
    /// <param name="logger">The logger instance to use for logging.</param>
    /// <param name="channel">The gRPC channel to use for the connection.</param>
    public GrpcInitializer(
        Channel channel,
        IPluginLogger logger)
    {
        this.Logger = logger;
        this.Channel = channel;

        this.Logger.Debug($"{nameof(GrpcInitializer)} constructor called.");
    }

    /// <summary>
    /// Asynchronously initializes the gRPC connection.
    /// </summary>
    /// <param name="token">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    public async Task InitializeAsync(CancellationToken token)
    {
        this.Logger.Debug($"{nameof(GrpcInitializer)}.{nameof(InitializeAsync)}() called.");

        try
        {
            this.Logger.Info($"Connecting to the gRPC server. (State: {this.Channel.State})");

            // If the channel is already connected, this will be a no-op.
            // ChannelStates:
            // - Idle: The channel has not yet connected.
            // - Connecting: The channel is connecting.
            // - Ready: The channel is connected and ready for use.
            // - TransientFailure: The channel has seen a failure but expects to recover.
            // - Shutdown: The channel has completed shutting down.
            // - FatalFailure: The channel has seen a failure that it cannot recover from.

            await this.Channel
                .ConnectAsync()
                .ConfigureAwait(false);

            this.Logger.Info($"Connected to the gRPC server. (State: {this.Channel.State})");
        }
        catch (OperationCanceledException ex) when (ex.CancellationToken == token)
        {
            this.Logger.Warn("The gRPC connection was canceled.");
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex, $"An error occurred while connecting to the gRPC server. (State: {this.Channel.State})");
        }

        if (this.Channel.State != ChannelState.Ready)
        {
            this.Logger.Warn($"The gRPC channel is not ready. (State: {this.Channel.State})");
        }

        this.Logger.Debug($"{nameof(GrpcInitializer)}.{nameof(InitializeAsync)}() completed.");
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
                this.Channel
                    .ShutdownAsync()
                    .ContinueWith(
                        task =>
                        {
                            if (task.IsFaulted)
                            {
                                this.Logger.Error(task.Exception, "Error shutting down the gRPC channel.");
                            }
                            else
                            {
                                this.Logger.Info("The gRPC channel has been shut down.");
                            }
                        },
                        TaskScheduler.Default);
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
