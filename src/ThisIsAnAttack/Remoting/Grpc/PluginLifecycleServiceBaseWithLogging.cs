using ThisIsAnAttack.Disposables;
using ThisIsAnAttack.Logging;
using ThisIsAnAttack.Services;

namespace ThisIsAnAttack.Remoting.Grpc;

public class PluginLifecycleServiceBaseWithLogging : PluginLifecycleServiceBase
{
    protected IPluginLogger Logger { get; }

    public PluginLifecycleServiceBaseWithLogging(
        IPluginLogger logger)
    {
        this.Logger = logger;
    }

    #region PluginLifecycleServiceBase

    protected override Task OnStartingAsync(CancellationToken stoppingToken)
    {
        base.OnStartingAsync(stoppingToken);

        this.Logger.InfoFormat(
            "Service starting: {0}",
            this.GetType().Name);

        return Task.CompletedTask;
    }

    protected override Task OnStoppedAsync(CancellationToken stoppingToken)
    {
        base.OnStoppedAsync(stoppingToken);

        this.Logger.InfoFormat(
            "Service stopped: {0}",
            this.GetType().Name);

        return Task.CompletedTask;
    }

    protected override Task OnCompletedAsync(CancellationToken stoppingToken)
    {
        base.OnCompletedAsync(stoppingToken);

        this.Logger.InfoFormat(
            "Service completed: {0}",
            this.GetType().Name);

        return Task.CompletedTask;
    }

    protected override Task OnCancelledAsync(
        OperationCanceledException exception)
    {
        base.OnCancelledAsync(exception);

        this.Logger.InfoFormat(
            "Service cancelled: {0}",
            this.GetType().Name);

        return Task.CompletedTask;
    }

    protected override Task OnFailedAsync(
        Exception ex,
        CancellationToken stoppingToken)
    {
        base.OnFailedAsync(ex, stoppingToken);

        this.Logger.InfoFormat(
            "Service failed: {0}",
            this.GetType().Name);

        return Task.CompletedTask;
    }

    protected override Task RunAsyncCore(CompositeDisposable disposables, CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }

    #endregion PluginLifecycleServiceBase
}
