using System.Runtime.CompilerServices;
using ThisIsAnAttack.Controllers;
using ThisIsAnAttack.Lifecycles;
using ThisIsAnAttack.Logging;

namespace ThisIsAnAttack.Services;

public class PluginDisablableServiceRunner : IPluginLifecycleAware, IDisposable
{
    private IPluginLifecycleService Service { get; }
    private IPluginLogger Logger { get; }

    public PluginDisablableServiceRunner(
        IPluginLifecycleService service,
        IPluginLogger logger)
    {
        this.Service = service;
        this.Logger = logger;

        this.Logger.Info("PluginDisablableServiceRunner created.");

        try
        {
            this.Logger.Info("Calling OnEnable() during the creation of PluginDisablableServiceRunner.");

            this.OnEnable();

            this.Logger.Info("OnEnable() completed during the creation of PluginDisablableServiceRunner.");
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex, "An exception occurred during the creation of PluginDisablableServiceRunner. The service is terminated and the plugin is likey to be inoperable.");
        }
    }

    #region IPluginLifecycleAware

    private PluginLifecycleContext? pluginLifecycleContext;

    public void OnEnable()
    {
        this.Logger.Info("OnEnable() called.");

        try
        {
            Interlocked.Exchange(
                ref this.pluginLifecycleContext,
                new PluginLifecycleContext(this.Service))
                ?.Dispose();

            this.Logger.Info("OnEnable() completed.");
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex, "An exception occurred during OnEnable(). The service is terminated and the plugin is likey to be inoperable.");
        }
    }

    public void OnDisable()
    {
        this.Logger.Info("OnDisable() called.");
        try
        {
            Interlocked.Exchange(
                ref this.pluginLifecycleContext,
                null)
                ?.Dispose();

            this.Logger.Info("OnDisable() completed.");
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex, "An exception occurred during OnDisable(). The service is terminated and the plugin is likey to be inoperable.");
        }
    }

    #endregion IPluginLifecycleAware

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
                Interlocked.Exchange(
                    ref this.pluginLifecycleContext,
                    null)
                    ?.Dispose();
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
