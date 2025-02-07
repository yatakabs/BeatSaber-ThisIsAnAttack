using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using ThisIsAnAttack.Logging;
using Zenject;

namespace ThisIsAnAttack.Services;

public interface IServiceManager
{
    void RegisterService<TService>()
        where TService : IService;

    void UnregisterService<TService>()
        where TService : IService;

    void RegisterService<TService>(string name)
        where TService : IService;

    void UnregisterService(string name);

    IEnumerable<ServiceRegistration> ServiceRegistrations { get; }

    ServiceStatus GetServiceStatus(string name);

    ServiceStatus GetServiceStatus<TService>()
        where TService : IService;

    IServiceController GetServiceControllerFor<TService>()
        where TService : IService;

    IServiceController GetServiceControllerFor(
        string name);
}

public record ServiceRegistration(
    string Name,
    IService Service);

public record ServiceStatus(
    IService Service,
    bool IsRunning);

public interface IServiceController
{
    Task StartServiceAsync(
        CancellationToken cancellationToken);

    Task StopServiceAsync();
}

public class ServiceManager : IServiceManager, IDisposable
{
    private IPluginLogger Logger { get; }

    private ConcurrentDictionary<string, ServiceRegistration> Services { get; } = new();

    public IEnumerable<ServiceRegistration> ServiceRegistrations => this.Services.Values;

    private DiContainer Container { get; }

    public ServiceManager(
        IPluginLogger logger,
        DiContainer container)
    {
        this.Logger = logger;
        this.Container = container;
    }

    public void RegisterService<TService>()
        where TService : IService
    {
        this.RegisterService<TService>(
            typeof(TService).Name);
    }

    public async void RegisterService<TService>(
        string name)
        where TService : IService
    {
        this.Logger.Debug($"Registering service: {name}");

        var isServieCreated = this.Services
            .TryAdd(name, new ServiceRegistration(
                name,
                this.Container.Resolve<TService>()));

        if (isServieCreated)
        {
            this.Logger.Debug($"Service registered: {name}");

            //Start the service
            this.Logger.Debug($"Starting service: {name}");
            var service = this.Services[name].Service;

            await service
                .StartAsync(default)
                .ConfigureAwait(false);

            this.Logger.Debug($"Service started: {name}");
        }
        else
        {
            this.Logger.Warn($"Service already registered: {name}");
        }
    }

    public void UnregisterService<TService>()
where TService : IService
    {
        this.UnregisterService(
            typeof(TService).Name);
    }

    public void UnregisterService(
        string name)
    {
        this.Logger.DebugFormat(
            "Unregistering service: {0}",
            name);
        if (this.Services.TryRemove(
            name,
            out var registration))
        {
            this.Logger.DebugFormat(
                "Service unregistered: {0}",
                name);
        }
        else
        {
            this.Logger.WarnFormat(
                "Service not found: {0}",
                name);
        }
    }

    public ServiceStatus GetServiceStatus(
        string name)
    {
        if (this.Services.TryGetValue(
            name,
            out var registration))
        {
            return new ServiceStatus(
                registration.Service,
                true);
        }

        throw new InvalidOperationException($"Service '{name}' not found.");
    }

    public ServiceStatus GetServiceStatus<TService>()
        where TService : IService
    {
        return this.GetServiceStatus(
            typeof(TService).Name);
    }

    public IServiceController GetServiceControllerFor<TService>()
        where TService : IService
    {
        return new ServiceController(
            this,
            typeof(TService).Name);
    }

    public IServiceController GetServiceControllerFor(
        string name)
    {
        return new ServiceController(
            this,
            name);
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
                var registrations = this.Services.Values.ToArray();
                foreach (var registration in registrations)
                {
                    if (registration.Service is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }

                this.Services.Clear();
            }
            this.isDisposed = true;
        }
    }

    #endregion IDisposable
}

public class ServiceController : IServiceController
{
    private ServiceManager ServiceManager { get; }
    private string ServiceName { get; }

    public ServiceController(
        ServiceManager serviceManager,
        string serviceName)
    {
        this.ServiceManager = serviceManager;
        this.ServiceName = serviceName;
    }

    public async Task StartServiceAsync(
        CancellationToken cancellationToken)
    {
        var status = this.ServiceManager.GetServiceStatus(
            this.ServiceName);

        if (status.IsRunning)
        {
            return;
        }

        if (status.Service is not IService service)
        {
            throw new InvalidOperationException($"Service '{this.ServiceName}' not found.");

        }
        await service
            .StartAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task StopServiceAsync()
    {
        var status = this.ServiceManager.GetServiceStatus(
            this.ServiceName);

        if (!status.IsRunning)
        {
            return;
        }

        if (status.Service is not IService service)
        {
            throw new InvalidOperationException($"Service '{this.ServiceName}' not found.");
        }

        await service
            .StopAsync()
            .ConfigureAwait(false);
    }

    public override string ToString() =>
        this.ServiceName;
}

public class ServicesInitializer : IInitializable
{
    private IServiceManager ServiceManager { get; }
    private IPluginLogger Logger { get; }

    public ServicesInitializer(
        IServiceManager serviceManager,
        IPluginLogger logger)
    {
        this.ServiceManager = serviceManager;
        this.Logger = logger;

        this.Logger.Debug($"{nameof(ServicesInitializer)} constructor called.");
    }

    public void Initialize()
    {
        this.Logger.Debug($"{nameof(ServicesInitializer)}.{nameof(Initialize)}() called.");

        this.ServiceManager.RegisterService<MainService>();
        this.Logger.Debug("MainService registered.");

        //this.ServiceManager.RegisterService<GameScoreStreamingService>();
        //this.Logger.Debug("GameScoreStreamingService registered.");

        this.Logger.Debug($"{nameof(ServicesInitializer)}.{nameof(Initialize)}() completed.");
    }
}
