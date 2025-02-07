using ThisIsAnAttack.Logging;

namespace ThisIsAnAttack.Services;

public abstract class SingleRunningServiceBaseWithLogging : SingleRunningServiceBase
{
    protected IPluginLogger Logger { get; }
    public SingleRunningServiceBaseWithLogging(
        IPluginLogger logger)
    {
        this.Logger = logger;
    }

    protected override Task OnStartingAsync(
        CancellationToken stoppingToken)
    {
        this.Logger.InfoFormat(
            "Service starting: {0}",
            this.GetType().Name);
        return Task.CompletedTask;
    }

    protected override Task OnStoppedAsync(
        CancellationToken stoppingToken)
    {
        this.Logger.InfoFormat(
            "Service stopped: {0}",
            this.GetType().Name);
        return Task.CompletedTask;
    }

    protected override Task OnCompletedAsync(
        CancellationToken stoppingToken)
    {
        this.Logger.InfoFormat(
            "Service completed: {0}",
            this.GetType().Name);
        return Task.CompletedTask;
    }

    protected override Task OnCancelledAsync(
        OperationCanceledException exception)
    {
        this.Logger.InfoFormat(
            "Service cancelled: {0}",
            this.GetType().Name);
        return Task.CompletedTask;
    }

    protected override Task OnFailedAsync(
        Exception exception,
        CancellationToken stoppingToken)
    {
        this.Logger.Error(
            exception,
            $"Service failed: {this.GetType().Name}");
        return Task.CompletedTask;
    }

}
