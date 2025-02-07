namespace ThisIsAnAttack.Lifecycles;

public interface IPluginLifecycleService
{
    Task RunAsync(CancellationToken stoppingToken);
}
