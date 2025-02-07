using System.Collections.Concurrent;

namespace ThisIsAnAttack.Monitors.Scoring;

public class MonitorCompositor : MonitorBase
{
    public ScoreMonitor ScoreMonitor { get; }
    public ComboMonitor ComboMonitor { get; }
    public EnergyMonitor GameEnergyMonitor { get; }
    public PauseMonitor PauseMonitor { get; }
    public MonitorCompositor(
        ScoreMonitor scoreMonitor,
        ComboMonitor comboMonitor,
        EnergyMonitor gameEnergyMonitor,
        PauseMonitor pauseMonitor)
    {
        this.ScoreMonitor = scoreMonitor;
        this.ComboMonitor = comboMonitor;
        this.GameEnergyMonitor = gameEnergyMonitor;
        this.PauseMonitor = pauseMonitor;

        this.Register(
            this.ScoreMonitor,
            sm => sm.ScoreChanged += this.OnScoreChanged,
            sm => sm.ScoreChanged -= this.OnScoreChanged);

        //this.Register(
        //    this.ScoreMonitor,
        //    sm => sm.MultiplierChanged += this.OnMultiplierChanged,
        //    sm => sm.MultiplierChanged -= this.OnMultiplierChanged);

        //this.Register(
        //    this.ScoreMonitor,
        //    sm => sm.MultiplierProgressChanged += this.OnMultiplierProgressChanged,
        //    sm => sm.MultiplierProgressChanged -= this.OnMultiplierProgressChanged);

        this.Register(
            this.ComboMonitor,
            cm => cm.ComboChanged += this.OnComboChanged,
            cm => cm.ComboChanged -= this.OnComboChanged);

        this.Register(
            this.GameEnergyMonitor,
            em => em.EnergyChanged += this.OnEnergyChanged,
            em => em.EnergyChanged -= this.OnEnergyChanged);
        this.Register(
            this.PauseMonitor,
            pm => pm.PauseSessionStarted += this.OnPauseSessionStarted,
            pm => pm.PauseSessionStarted -= this.OnPauseSessionStarted);

        this.Register(
            this.PauseMonitor,
            pm => pm.PauseSessionEnded += this.OnPauseSessionEnded,
            pm => pm.PauseSessionEnded -= this.OnPauseSessionEnded);
    }

    private ConcurrentQueue<Func<MapPlayProgress, MapPlayProgress>> ProgressUpdateQueue { get; } = new();
    private MapPlayProgress progress = new(
        NoteCut: 0,
        NoteMissed: 0,
        NoteBadCut: 0,
        BombCut: 0,
        WallHit: 0,
        TotalWallHitDuration: TimeSpan.Zero,
        TotalPauseDuration: TimeSpan.Zero,
        Multiplier: 1,
        MultiplierProgress: 0,
        Combo: 0,
        Energy: 0,
        PoseCount: 0);

    public MapPlayProgress CurrentProgress => this.progress;

    private void EnqueProgrssUpdate(Func<MapPlayProgress, MapPlayProgress> update)
    {
        this.ProgressUpdateQueue.Enqueue(update);
    }

    public bool UpdateProgress()
    {
        var updatingProgress = Volatile.Read(ref this.progress);
        var updated = false;

        while (this.ProgressUpdateQueue.TryDequeue(out var update))
        {
            updatingProgress = update(updatingProgress);
            updated = true;
        }

        Volatile.Write(ref this.progress, updatingProgress);

        if (updated)
        {
            var args = new ProgressChangedEventArgs(updatingProgress);
            this.ProgressChanged?.Invoke(this, args);
        }

        return updated;
    }

    private void OnScoreChanged(object sender, ScoreChangedEventArgs e)
    {
        this.EnqueProgrssUpdate(progress =>
        {
            return progress with
            {
            };
        });
    }

    private void OnMultiplierChanged(object sender, MultiplierChangedEventArgs e)
    {
        this.EnqueProgrssUpdate(progress => progress with
        {
            Multiplier = e.Multiplier,
            MultiplierProgress = e.Progress
        });
    }

    private void OnMultiplierProgressChanged(object sender, MultiplierProgressChangedEventArgs e)
    {
        this.EnqueProgrssUpdate(progress => progress with
        {
            Multiplier = (int)e.Multiplier,
            MultiplierProgress = e.Progress
        });
    }

    private void OnComboChanged(object sender, ComboChangedEventArgs e)
    {
        this.EnqueProgrssUpdate(progress => progress with
        {
            Combo = e.LatestSnapshot.Combo,
        });
    }

    private void OnEnergyChanged(object sender, EnergyChangedEventArgs e)
    {
        this.EnqueProgrssUpdate(progress => progress with
        {
            Energy = e.LatestSnapshot.Energy,
        });
    }

    private void OnPauseSessionStarted(object sender, PauseSessionStartedEventArgs e)
    {
        this.EnqueProgrssUpdate(progress => progress with
        {
            TotalPauseDuration = progress.TotalPauseDuration + e.Session.Duration,
        });
    }

    private void OnPauseSessionEnded(object sender, PauseSessionEndedEventArgs e)
    {
        this.EnqueProgrssUpdate(progress => progress with
        {
            TotalPauseDuration = progress.TotalPauseDuration + e.TotalPauseDuration,
        });
    }

    public event EventHandler<ProgressChangedEventArgs>? ProgressChanged;
}

public class ProgressChangedEventArgs : EventArgs
{
    public MapPlayProgress GameProgress { get; }

    public ProgressChangedEventArgs(MapPlayProgress gameProgress)
    {
        this.GameProgress = gameProgress;
    }
}
