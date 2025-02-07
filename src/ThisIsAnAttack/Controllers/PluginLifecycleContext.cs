using System.Runtime.CompilerServices;
using ThisIsAnAttack.Disposables;
using ThisIsAnAttack.Lifecycles;

namespace ThisIsAnAttack.Controllers;

public sealed class PluginLifecycleContext : IDisposable
{
    public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();
    public CompositeDisposable CompositeDisposable { get; } = new CompositeDisposable();
    public IPluginLifecycleService Service { get; }
    public Task ServiceTask { get; }

    public PluginLifecycleContext(
        IPluginLifecycleService service)
    {
        this.Service = service;

        this.CancellationTokenSource.AddTo(this.CompositeDisposable);

        this.CompositeDisposable
            .Register(() => this.CancellationTokenSource.Cancel());

        this.ServiceTask = this.Service.RunAsync(this.CancellationTokenSource.Token);
    }

    public async Task TerminateAsync(
        bool waitForCompletion = true,
        CancellationToken cancellationToken = default)
    {
        this.CancellationTokenSource.Cancel();

        if (waitForCompletion)
        {
            await this
                .WaitForCompletionAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Terminates the service and ignores the exception if the operation is cancelled.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for waiting for the service to terminate.</param>
    /// <remarks>
    /// This method never throws an exception unless it is caused by this context class itself, or by the service which is terminated.
    /// </remarks>
    public async Task TerminateSafeAsync(
        Action<Exception>? onError = null,
        Action? onCompleted = null,
        Action? onCancelled = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await this
                .TerminateAsync(
                    waitForCompletion: true,
                    cancellationToken)
                .ConfigureAwait(false);

            onCompleted?.Invoke();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Just rethrow the exception as this is requested by the caller.
            throw;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // Invoke the onCancelled action as the service is cancelled.
            onCancelled?.Invoke();
        }
        catch (Exception ex)
        {
            // Invoke the onError action as the service is cancelled.
            onError?.Invoke(ex);
        }
    }

    public async Task WaitForCompletionAsync(
        CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            this.CancellationTokenSource.Token,
            cancellationToken);

        try
        {
            var ret = await Task.WhenAny(
                this.ServiceTask,
                Task.Delay(Timeout.Infinite, linkedCts.Token))
                .ConfigureAwait(false);

            // If the service task is not the one that completed, then the cancellation token was triggered.
            if (ret != this.ServiceTask)
            {
                linkedCts.Token.ThrowIfCancellationRequested();
            }

            // If the service task is the one that completed, then rethrow the exception.
            await this.ServiceTask
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Rethrow the exception as this is requested by the caller.
            throw;
        }
        catch (OperationCanceledException)
        {
            // Ignore the exception as this is not requested by the caller.
            // The service itself is 
        }
    }

    #region IDisposable

    private volatile bool isDisposed;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Dispose(bool disposing)
    {
        if (!this.isDisposed)
        {
            if (disposing)
            {
                this.CancellationTokenSource.Cancel();
                this.CompositeDisposable.Dispose();
            }

            this.isDisposed = true;
        }
    }
    #endregion IDisposable
}
