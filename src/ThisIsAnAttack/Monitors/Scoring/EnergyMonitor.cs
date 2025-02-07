using ThisIsAnAttack.Logging;

namespace ThisIsAnAttack.Monitors.Scoring;

public class EnergyMonitor : MonitorBase
{
    private IPluginLogger Logger { get; }
    public GameEnergyCounter GameEnergyCounter { get; }

    public EnergySnapshot LatestSnapshot { get; private set; } = EnergySnapshot.Empty with
    {
        Timestamp = DateTimeOffset.Now
    };

    public EnergyMonitor(GameEnergyCounter gameEnergyCounter, IPluginLogger logger)
    {
        this.GameEnergyCounter = gameEnergyCounter;
        this.Logger = logger;

        this.Logger.Debug($"{nameof(EnergyMonitor)} constructor called.");

        this.Register(
            this.GameEnergyCounter,
            gc => gc.gameEnergyDidReach0Event += this.OnGameEnergyDidReach0,
            gc => gc.gameEnergyDidReach0Event -= this.OnGameEnergyDidReach0);

        this.Register(
            this.GameEnergyCounter,
            gc => gc.gameEnergyDidChangeEvent += this.OnGameEnergyDidChange,
            gc => gc.gameEnergyDidChangeEvent -= this.OnGameEnergyDidChange);

        this.Register(
            this.GameEnergyCounter,
            gc => gc.didInitEvent += this.OnGameEnergyDidInit,
            gc => gc.didInitEvent -= this.OnGameEnergyDidInit);

        var initialSnapshot = new EnergySnapshot
        {
            Timestamp = DateTimeOffset.Now,
            Energy = this.GameEnergyCounter.energy,
            BatteryEnergy = this.GameEnergyCounter.batteryEnergy,
            IsBatteryEnergy = this.GameEnergyCounter.energyType == GameplayModifiers.EnergyType.Battery,
        };

        this.LatestSnapshot = initialSnapshot;

        this.Logger.Debug($"Initial snapshot: {initialSnapshot}");
    }

    private bool HandleChanged(
        EnergySnapshot newSnapshot)
    {
        var previousSnapshot = this.LatestSnapshot;

        var isChanged = newSnapshot.WithoutTimestamp() != previousSnapshot.WithoutTimestamp();
        if (isChanged)
        {
            this.Logger.Debug($"Energy changed. Energy: {newSnapshot.Energy}");

            this.LatestSnapshot = newSnapshot;

            var args = new EnergyChangedEventArgs(newSnapshot, previousSnapshot);
            this.EnergyChanged?.Invoke(this, args);
        }

        return false;
    }

    private void OnGameEnergyDidReach0()
    {
        this.Logger.Debug("Energy reached 0.");

        var snapshot = new EnergySnapshot
        {
            Timestamp = DateTimeOffset.Now,
            Energy = 0,
            BatteryEnergy = this.GameEnergyCounter.batteryEnergy,
            IsBatteryEnergy = this.GameEnergyCounter.energyType == GameplayModifiers.EnergyType.Battery,
        };

        this.HandleChanged(snapshot);

        var args = new EnergyReached0EventArgs(snapshot);
        this.EnergyReached0?.Invoke(this, args);
    }

    private void OnGameEnergyDidChange(float energy)
    {
        this.Logger.Debug($"Energy changed. Energy: {energy}");
        var snapshot = new EnergySnapshot
        {
            Timestamp = DateTimeOffset.Now,
            Energy = energy,
            BatteryEnergy = this.GameEnergyCounter.batteryEnergy,
            IsBatteryEnergy = this.GameEnergyCounter.energyType == GameplayModifiers.EnergyType.Battery,
        };

        this.HandleChanged(snapshot);
    }

    private void OnGameEnergyDidInit()
    {
        this.Logger.Debug("Energy initialized.");

        var snapshot = new EnergySnapshot
        {
            Timestamp = DateTimeOffset.Now,
            Energy = this.GameEnergyCounter.energy,
            BatteryEnergy = this.GameEnergyCounter.batteryEnergy,
            IsBatteryEnergy = this.GameEnergyCounter.energyType == GameplayModifiers.EnergyType.Battery,
        };

        this.HandleChanged(snapshot);

        var args = new EnergyInitializedEventArgs(snapshot);
        this.EnergyInitialized?.Invoke(this, args);
    }

    public event EventHandler<EnergyChangedEventArgs>? EnergyChanged;
    public event EventHandler<EnergyInitializedEventArgs>? EnergyInitialized;
    public event EventHandler<EnergyReached0EventArgs>? EnergyReached0;

}

public class EnergyChangedEventArgs : EventArgs
{
    public EnergySnapshot LatestSnapshot { get; }
    public EnergySnapshot PreviousSnapshot { get; }
    public EnergyChangedEventArgs(EnergySnapshot latestSnapshot, EnergySnapshot previousSnapshot)
    {
        this.LatestSnapshot = latestSnapshot;
        this.PreviousSnapshot = previousSnapshot;
    }
}

public class EnergyInitializedEventArgs : EventArgs
{
    public EnergySnapshot Snapshot { get; }
    public EnergyInitializedEventArgs(EnergySnapshot snapshot)
    {
        this.Snapshot = snapshot;
    }
}

public class EnergyReached0EventArgs : EventArgs
{
    public EnergySnapshot Snapshot { get; }
    public EnergyReached0EventArgs(EnergySnapshot snapshot)
    {
        this.Snapshot = snapshot;
    }
}

public record EnergySnapshot
{
    public required DateTimeOffset Timestamp { get; init; }
    public required float Energy { get; init; }
    public required int BatteryEnergy { get; init; }
    public required bool IsBatteryEnergy { get; init; }

    public EnergySnapshot WithoutTimestamp()
    {
        return this with
        {
            Timestamp = DateTimeOffset.MinValue
        };
    }

    public static EnergySnapshot Empty { get; } = new EnergySnapshot
    {
        Timestamp = DateTimeOffset.MinValue,
        Energy = 0,
        BatteryEnergy = 0,
        IsBatteryEnergy = false,
    };
}
