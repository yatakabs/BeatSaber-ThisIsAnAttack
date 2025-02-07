namespace ThisIsAnAttack.Services;

public class ServiceBase : IService
{
    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    public virtual Task StopAsync()
    {
        return Task.CompletedTask;
    }
}
