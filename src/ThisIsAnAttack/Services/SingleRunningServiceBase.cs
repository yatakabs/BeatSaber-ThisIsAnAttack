using System.Runtime.CompilerServices;
using ThisIsAnAttack.Disposables;

namespace ThisIsAnAttack.Services;

public record SingleRunningServiceContext(
    CancellationTokenSource CancellationTokenSource,
    CompositeDisposable Disposables,
    Task Task);

public abstract class SingleRunningServiceBase : ServiceBase, IDisposable
{
    private volatile SingleRunningServiceContext? context;
    private readonly object updateLock = new();

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        lock (this.updateLock)
        {
            if (this.context != null)
            {
                throw new InvalidOperationException("The service is already running.");
            }

            var cts = new CancellationTokenSource();
            var disposables = new CompositeDisposable();

            var task = Task.Run(
                async () =>
                {
                    try
                    {
                        await this
                            .RunAsync(cts.Token)
                            .ConfigureAwait(false);
                    }
                    finally
                    {
                        cts.Dispose();
                        disposables.Dispose();
                    }
                },
                cts.Token);

            this.context = new SingleRunningServiceContext(
                cts,
                disposables,
                task);
        }

        return Task.CompletedTask;
    }

    public override Task StopAsync()
    {
        SingleRunningServiceContext? currentContext;
        lock (this.updateLock)
        {
            currentContext = this.context;
            this.context = null;
        }

        if (currentContext != null)
        {
            currentContext.CancellationTokenSource.Cancel();
            return currentContext.Task;
        }

        return Task.CompletedTask;
    }

    public async Task RunAsync(CancellationToken stoppingToken)
    {
        using var disposables = new CompositeDisposable();

        try
        {
            await this
                .OnStartingAsync(stoppingToken)
                .ConfigureAwait(false);

            await this
                .RunAsyncCore(
                    disposables,
                    stoppingToken)
                .ConfigureAwait(false);

            await this
                .OnCompletedAsync(stoppingToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException ex) when (ex.CancellationToken != stoppingToken)
        {
            await this
                .OnCancelledAsync(ex)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await this
                .OnFailedAsync(ex, stoppingToken)
                .ConfigureAwait(false);
        }
        finally
        {
            await this
                .OnStoppedAsync(stoppingToken)
                .ConfigureAwait(false);
        }
    }

    protected abstract Task RunAsyncCore(
        CompositeDisposable disposables,
        CancellationToken stoppingToken);

    #region State event handlers

    protected virtual Task OnStartingAsync(
        CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnStoppedAsync(
        CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnCompletedAsync(
        CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnCancelledAsync(
        OperationCanceledException exception)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnFailedAsync(
        Exception exception,
        CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    #endregion

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
                // Dispose managed resources here.
                _ = this.StopAsync();
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
