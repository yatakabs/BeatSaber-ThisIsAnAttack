using SiraUtil.Zenject;
using ThisIsAnAttack.Logging;
using ThisIsAnAttack.Monitors.Scoring;
using ThisIsAnAttack.Services;

namespace ThisIsAnAttack.Installers;

/// <summary>
/// Installs the scoring system.
/// Installation location must be <see cref="Location.App"/>.
/// </summary>
public class ScoringPlayerInstaller : Zenject.Installer
{
    private IPluginLogger Logger { get; }

    public ScoringPlayerInstaller(IPluginLogger logger)
    {
        this.Logger = logger;
        this.Logger.Debug($"{nameof(ScoringPlayerInstaller)} constructor called.");
    }

    public override void InstallBindings()
    {
        this.Logger.Debug($"{nameof(ScoringPlayerInstaller)} called.");

        this.Container.Bind<ScoreMonitor>()
            .ToSelf()
            .AsTransient();

        this.Container
            .Bind<ComboMonitor>()
            .ToSelf()
            .AsTransient();

        this.Container.Bind<PauseMonitor>()
            .ToSelf()
            .AsTransient();

        this.Container
            .Bind<EnergyMonitor>()
            .ToSelf()
            .AsTransient();

        this.Container
            .BindInterfacesAndSelfTo<GameScoreStreamingService>()
            .AsTransient();

        this.Container
            .BindInterfacesAndSelfTo<ScoreSubmissionInitializer>()
            .AsSingle();

        this.Logger.Debug($"{nameof(ScoringPlayerInstaller)} completed.");
    }
}
