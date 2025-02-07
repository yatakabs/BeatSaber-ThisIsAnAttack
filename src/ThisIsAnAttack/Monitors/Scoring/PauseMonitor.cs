using IPA.Utilities;
using ThisIsAnAttack.Logging;

namespace ThisIsAnAttack.Monitors.Scoring;

public sealed class PauseMonitor : MonitorBase
{
    private IPluginLogger Logger { get; }
    public PauseController PauseController { get; }
    public PauseStateSnapshot LatestSnapshot { get; private set; } = PauseStateSnapshot.Empty;

    public PauseMonitor(
        PauseController pauseController,
        IPluginLogger logger)
    {
        this.PauseController = pauseController;
        this.Logger = logger;

        this.Logger.Debug($"{nameof(PauseMonitor)} constructor called.");

        this.Register(
            this.PauseController,
            pc => pc.didPauseEvent += this.OnPause,
            pc => pc.didPauseEvent -= this.OnPause);

        this.Register(
            this.PauseController,
            pc => pc.didResumeEvent += this.OnResume,
            pc => pc.didResumeEvent -= this.OnResume);

        this.Register(
            this.PauseController,
            pc => pc.didReturnToMenuEvent += this.OnReturnToMenu,
            pc => pc.didReturnToMenuEvent -= this.OnReturnToMenu);

        this.Logger.Debug($"Current state: {new
        {
            this.PauseController.isActiveAndEnabled,
            this.PauseController.name,
            this.PauseController.enabled,
            this.PauseController.tag,
            this.PauseController.useGUILayout,
            this.PauseController.wantsToPause,
        }}");

        var isPaused = this.PauseController.GetField<bool, PauseController>("_paused");

        var currentPauseSession = isPaused
            ? new PauseSession
            {
                State = PauseState.Paused,
                StartedAt = DateTimeOffset.Now,
                Duration = TimeSpan.Zero,
            }
            : null;

        this.LatestSnapshot = new PauseStateSnapshot
        {
            Timestamp = DateTimeOffset.Now,
            IsPaused = isPaused,
            PauseCount = isPaused ? 1 : 0,
            TotalPauseDuration = TimeSpan.Zero,
            Current = currentPauseSession,
            History = [],
        };

        this.Logger.Debug($"Initial snapshot: {this.LatestSnapshot}");
    }

    private bool HandleUpdate(
        PauseStateSnapshot newSnapshot)
    {
        this.Logger.Debug($"New snapshot: {newSnapshot}");

        var oldSnapshot = this.LatestSnapshot;

        var isChanged = oldSnapshot.WithoutTimestamp() != newSnapshot.WithoutTimestamp();
        if (isChanged)
        {
            this.Logger.Debug("Snapshot changed.");
            this.LatestSnapshot = newSnapshot;

            var args = new PauseStateChangedEventArgs(newSnapshot, oldSnapshot);
            this.PauseStateChanged?.Invoke(this, args);

            return true;
        }

        this.Logger.Debug("Snapshot not changed.");
        return false;
    }

    private void OnPause()
    {
        this.Logger.Debug("OnPause");

        var currentPauseSession = this.LatestSnapshot.Current;
        if (currentPauseSession is not null)
        {
            this.Logger.Warn($"Pause session is already started: {this.LatestSnapshot}");

            this.Logger.Warn("Force to end the current pause session and start a new one.");

            currentPauseSession = currentPauseSession with
            {
                State = PauseState.Ended,
                Duration = DateTimeOffset.Now - currentPauseSession.StartedAt,
            };

            var resumeSnapshot = this.LatestSnapshot with
            {
                IsPaused = false,
                Current = null,
                TotalPauseDuration = this.LatestSnapshot.TotalPauseDuration + currentPauseSession.Duration,
                History = [.. this.LatestSnapshot.History, currentPauseSession],
            };

            if (this.HandleUpdate(resumeSnapshot))
            {
                this.Logger.Debug($"Pause session ended (force): {this.LatestSnapshot}");

                var resumeArgs = new PauseSessionEndedEventArgs(
                    session: currentPauseSession,
                    totalPauseDuration: this.LatestSnapshot.TotalPauseDuration,
                    pauseCount: this.LatestSnapshot.PauseCount);

                this.PauseSessionEnded?.Invoke(this, resumeArgs);
            }
            else
            {
                this.Logger.Warn($"Pause state not changed: {this.LatestSnapshot}");
            }
        }

        currentPauseSession = new PauseSession
        {
            State = PauseState.Paused,
            StartedAt = DateTimeOffset.Now,
            Duration = TimeSpan.Zero,
        };

        var newSnapshot = this.LatestSnapshot with
        {
            IsPaused = true,
            Current = currentPauseSession,
            PauseCount = this.LatestSnapshot.PauseCount + 1,
        };

        if (this.HandleUpdate(newSnapshot))
        {
            this.Logger.Debug($"Pause session started: {this.LatestSnapshot}");

            var args = new PauseSessionStartedEventArgs(currentPauseSession);
            this.PauseSessionStarted?.Invoke(this, args);
        }
        else
        {
            this.Logger.Warn($"Pause state not changed: {this.LatestSnapshot}");
        }
    }

    private void OnResume()
    {
        this.Logger.Debug("OnResume");

        var currentPauseSession = this.LatestSnapshot.Current;
        if (currentPauseSession is null)
        {
            this.Logger.Warn("Pause session is not started. Creating an empty session as a workaround.");
            currentPauseSession = new PauseSession
            {
                State = PauseState.Paused,
                StartedAt = DateTimeOffset.Now,
                Duration = TimeSpan.Zero,
            };

            var pausedSnapshot = this.LatestSnapshot with
            {
                IsPaused = true,
                Current = currentPauseSession,
                PauseCount = this.LatestSnapshot.PauseCount + 1,
            };

            if (this.HandleUpdate(pausedSnapshot))
            {
                this.Logger.Debug($"Pause session started (workaround): {this.LatestSnapshot}");
                var args = new PauseSessionStartedEventArgs(currentPauseSession);
                this.PauseSessionStarted?.Invoke(this, args);
            }
            else
            {
                this.Logger.Warn($"Pause state not changed: {this.LatestSnapshot}");
            }
        }

        currentPauseSession = currentPauseSession with
        {
            State = PauseState.Ended,
            Duration = DateTimeOffset.Now - currentPauseSession.StartedAt,
        };

        var newSnapshot = this.LatestSnapshot with
        {
            IsPaused = false,
            Current = null,
            TotalPauseDuration = this.LatestSnapshot.TotalPauseDuration + currentPauseSession.Duration,
            History = [.. this.LatestSnapshot.History, currentPauseSession],
        };

        if (this.HandleUpdate(newSnapshot))
        {
            this.Logger.Debug($"Pause session ended: {this.LatestSnapshot}");
            var args = new PauseSessionEndedEventArgs(
                session: currentPauseSession,
                totalPauseDuration: this.LatestSnapshot.TotalPauseDuration,
                pauseCount: this.LatestSnapshot.PauseCount);
            this.PauseSessionEnded?.Invoke(this, args);
        }
        else
        {
            this.Logger.Warn($"Pause state not changed: {this.LatestSnapshot}");
        }
    }

    private void OnReturnToMenu()
    {
        this.Logger.Debug("Return to menu.");

        var currentPauseSession = this.LatestSnapshot.Current
            ?? throw new InvalidOperationException("Pause session is not started.");

        currentPauseSession = currentPauseSession with
        {
            State = PauseState.Ended,
            Duration = DateTimeOffset.Now - currentPauseSession.StartedAt,
        };

        var last = this.LatestSnapshot;
        this.LatestSnapshot = last with
        {
            IsPaused = false,
            Current = null,
            TotalPauseDuration = last.TotalPauseDuration + currentPauseSession.Duration,
            History = [.. last.History, currentPauseSession],
        };

        var args = new PauseSessionEndedEventArgs(
            session: currentPauseSession,
            totalPauseDuration: this.LatestSnapshot.TotalPauseDuration,
            pauseCount: this.LatestSnapshot.PauseCount);

        this.PauseSessionEnded?.Invoke(this, args);

        this.Logger.Debug($"Pause session ended (return to menu): {this.LatestSnapshot}");
    }

    public event EventHandler<PauseSessionStartedEventArgs>? PauseSessionStarted;
    public event EventHandler<PauseSessionEndedEventArgs>? PauseSessionEnded;
    public event EventHandler<PauseStateChangedEventArgs>? PauseStateChanged;
}

public class PauseStateChangedEventArgs : EventArgs
{
    public PauseStateSnapshot LatestSnapshot { get; }
    public PauseStateSnapshot PreviousSnapshot { get; }
    public PauseStateChangedEventArgs(
        PauseStateSnapshot latestSnapshot,
        PauseStateSnapshot previousSnapshot)
    {
        this.LatestSnapshot = latestSnapshot;
        this.PreviousSnapshot = previousSnapshot;
    }
}
