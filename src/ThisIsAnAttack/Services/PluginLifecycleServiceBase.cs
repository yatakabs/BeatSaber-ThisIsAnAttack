using System.Runtime.CompilerServices;
using ThisIsAnAttack.Disposables;
using ThisIsAnAttack.Lifecycles;

namespace ThisIsAnAttack.Services;

public abstract class PluginLifecycleServiceBase : IPluginLifecycleService, IDisposable
{
    private CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();
    protected CompositeDisposable Disposables { get; } = new CompositeDisposable();

    //#region IService

    //public async Task StartAsync(CancellationToken cancellationToken)
    //{
    //}

    //Task StopAsync();

    //#endregion

    public async Task RunAsync(
        CancellationToken stoppingToken)
    {
        try
        {
            using var disposables = new CompositeDisposable();

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                this.CancellationTokenSource.Token,
                stoppingToken);

            await this
                .OnStartingAsync(linkedCts.Token)
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
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Ignore the exception as the operation is cancelled.
            await this
                .OnCancelledAsync(new OperationCanceledException(stoppingToken))
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException ex)
        {
            // Ignore the exception as
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

    #region Lifecycle event methods

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

    protected virtual Task OnFailedAsync(
        Exception ex,
        CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnCancelledAsync(
        OperationCanceledException exception)
    {
        return Task.CompletedTask;
    }

    protected virtual Task StoppedAsync(
        CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    #endregion Lifecycle event methods

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
                this.Disposables.Dispose();
            }

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
