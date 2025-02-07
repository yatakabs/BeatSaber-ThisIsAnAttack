using ThisIsAnAttack.Disposables;
using ThisIsAnAttack.Logging;
using ThisIsAnAttack.Monitors;
using Zenject;

namespace ThisIsAnAttack.Services;

public class MainService : SingleRunningServiceBaseWithLogging
{
    private new IPluginLogger Logger { get; }
    private SceneChangeMonitor SceneChangeMonitor { get; }
    private DiContainer Container { get; }

    public MainService(
        SceneChangeMonitor sceneChangeMonitor,
        DiContainer container,
        IPluginLogger logger)
        : base(logger)
    {
        this.SceneChangeMonitor = sceneChangeMonitor;
        this.Container = container;
        this.Logger = logger;
    }

    protected override Task RunAsyncCore(
        CompositeDisposable disposables,
        CancellationToken stoppingToken)
    {
        this.Logger.Info($"{nameof(MainService)} started. Stopping token: {stoppingToken.GetHashCode()}");

        this.Logger.Info("Registering scene bound actions for the MenuCore scene.");
        this.SceneChangeMonitor
            .RegisterSceneBoundActions(
                scenePredicate: scene =>
                {
                    var isMenuCore = scene.name == "MenuCore";
                    this.Logger.Debug($"Scene predicate: {scene.name} -> {isMenuCore}");
                    return isMenuCore;
                },
                onStart: scene =>
                {
                    this.Logger.Info("Menu scene loaded.");
                },
                onComplete: scene =>
                {
                    this.Logger.Info("Menu scene unloaded.");
                })
            .AddTo(disposables);

        // Wait for the stopping token to be triggered.
        this.Logger.Info("Waiting for the stopping token to be triggered.");
        return Task.Delay(Timeout.Infinite, stoppingToken);
    }
}

public interface IService
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync();
}

public record GameProgress(
    TimeSpan SongPosition,
    TimeSpan SongDuration,
    bool IsPlaying,
    bool IsPaused,
    bool IsStopped,
    bool IsFinished);

public interface IGameProgressSubscriber
{
    void OnGameProgressChanged(GameProgress progress);
    void OnGameProgressReset();
}
