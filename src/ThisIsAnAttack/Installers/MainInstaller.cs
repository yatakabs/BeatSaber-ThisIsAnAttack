using ThisIsAnAttack.Logging;
using ThisIsAnAttack.Monitors;
using ThisIsAnAttack.Services;

namespace ThisIsAnAttack.Installers;
public class MainInstaller : Zenject.Installer<MainInstaller>
{
    private IPluginLogger Logger { get; }

    public MainInstaller(
        IPluginLogger logger)
    {
        this.Logger = logger;
        this.Logger.Debug("ScoreMonitorInstaller ctor called.");
    }

    public override void InstallBindings()
    {
        this.Logger.Debug("MainInstaller.InstallBindings() called.");

        this.Container
            .BindInterfacesAndSelfTo<SceneChangeMonitor>()
            .AsSingle();

        this.Container
            .BindInterfacesAndSelfTo<MainService>()
            .AsSingle();

        this.Container
            .BindInterfacesAndSelfTo<ServiceManager>()
            .AsSingle();

        this.Container
            .BindInterfacesAndSelfTo<ServicesInitializer>()
            .AsSingle();

        //this.Container
        //    .Bind<PluginDisablableServiceRunner>()
        //    .ToSelf()
        //    .AsSingle()
        //    .NonLazy();

        this.Logger.Debug("MainInstaller.InstallBindings() completed.");
    }
}

//public class CustomHttpClientHandler : HttpClientHandler
//{
//    public CustomHttpClientHandler()
//    {
//        // 全てのサーバ証明書を許容（※セキュリティ上のリスクに注意）
//        this.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

//        // 
//    }
//}

