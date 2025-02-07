using System.Runtime.CompilerServices;
using ThisIsAnAttack.Logging;

namespace ThisIsAnAttack.Services;

public class ServiceRunner : IDisposable
{
    private IService Service { get; }
    private IPluginLogger Logger { get; }

    public ServiceRunner(
        IService service,
        IPluginLogger logger)
    {
        this.Service = service;
        this.Logger = logger;
    }

    public async Task StartAsync(
        CancellationToken token)
    {
        this.Logger.Debug($"{nameof(ServiceRunner)}.{nameof(StartAsync)}() called.");
        try
        {
            await this.Service
                .StartAsync(token)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException ex) when (ex.CancellationToken == token)
        {
            this.Logger.Warn("The service was canceled.");
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex, $"An error occurred while starting the service.");
        }
    }

    public async Task StopAsync()
    {
        this.Logger.Debug($"{nameof(ServiceRunner)}.{nameof(StopAsync)}() called.");

        try
        {
            await this.Service
                .StopAsync()
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex, $"An error occurred while stopping the service.");
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

                this.Logger.Debug($"{nameof(ServiceRunner)} disposing.");

                this.StopAsync()
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            this.Logger.Error(
                                t.Exception,
                                $"{nameof(ServiceRunner)} failed to stop service.");
                        }

                        if (this.Service is IDisposable disposableService)
                        {
                            disposableService.Dispose();
                        }

                        this.Logger.Debug($"{nameof(ServiceRunner)} disposed.");
                    });
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
