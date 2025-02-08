using ThisIsAnAttack.Logging;

namespace ThisIsAnAttack.Monitors.Scoring;

public class ScoreMonitor : MonitorBase
{
    public IPluginLogger Logger { get; }
    public IScoreController ScoreController { get; }

    public TimestampedScoreSnapshot LatestSnapshot { get; private set; } = new TimestampedScoreSnapshot
    {
        Timestamp = default,
        Scores = new ScoreSnapshot
        {
            Multiplier = 1,
            MultiplierProgress = 0,
            Score = 0,
            MaxScore = 0,
            ModifiedScore = 0,
            MaxModifiedScore = 0
        }
    };

    public ScoreMonitor(
        IScoreController scoreController,
        IPluginLogger logger)
    {
        this.ScoreController = scoreController;
        this.Logger = logger;

        this.Logger.Debug($"{nameof(ScoreMonitor)} constructor called.");

        this.Register(
            this.ScoreController,
            sc => sc.scoreDidChangeEvent += this.ScoreController_scoreDidChangeEvent,
            sc => sc.scoreDidChangeEvent -= this.ScoreController_scoreDidChangeEvent);

        this.Register(
            this.ScoreController,
            sc => sc.multiplierDidChangeEvent += this.ScoreController_multiplierDidChangeEvent,
            sc => sc.multiplierDidChangeEvent -= this.ScoreController_multiplierDidChangeEvent);

        var snapshot = new ScoreSnapshot
        {
            Score = this.ScoreController.multipliedScore,
            MaxScore = this.ScoreController.immediateMaxPossibleMultipliedScore,
            ModifiedScore = this.ScoreController.modifiedScore,
            MaxModifiedScore = this.ScoreController.immediateMaxPossibleModifiedScore,
            Multiplier = 1,
            MultiplierProgress = 0f,
        };

        var initialSnapshotWithTimestamp = new TimestampedScoreSnapshot
        {
            Timestamp = DateTimeOffset.Now,
            Scores = snapshot
        };

        this.Logger.Debug($"Initial snapshot: {initialSnapshotWithTimestamp}");
        this.LatestSnapshot = initialSnapshotWithTimestamp;
    }

    private void ScoreController_multiplierDidChangeEvent(int multiplier, float progress)
    {
        this.Logger.Debug($"Multiplier Changed. Multiplier: {multiplier}, Progress: {progress}");

        var newSnapshot = this.LatestSnapshot.Scores with
        {
            Multiplier = multiplier,
            MultiplierProgress = progress,
        };

        this.UpdateSnapshot(newSnapshot);
    }

    private void ScoreController_scoreDidChangeEvent(int score, int modifiedScore)
    {
        this.Logger.Debug($"Score Changed. Score: {score}, Modified: {modifiedScore}");

        var newSnapshot = new ScoreSnapshot
        {
            Score = this.ScoreController.multipliedScore,
            MaxScore = this.ScoreController.immediateMaxPossibleMultipliedScore,
            ModifiedScore = this.ScoreController.modifiedScore,
            MaxModifiedScore = this.ScoreController.immediateMaxPossibleModifiedScore,
            MultiplierProgress = this.LatestSnapshot.Scores.MultiplierProgress,
            Multiplier = this.LatestSnapshot.Scores.Multiplier,
        };

        this.Logger.Debug($"New snapshot: {newSnapshot}");
        this.UpdateSnapshot(newSnapshot);
    }

    private void UpdateSnapshot(ScoreSnapshot newSnapshot)
    {
        var lastSnapshotWithTimestamp = this.LatestSnapshot;
        var changed = this.LatestSnapshot.Scores != newSnapshot;

        if (changed)
        {
            var newSnapshotWithTimestamp = new TimestampedScoreSnapshot
            {
                Timestamp = DateTimeOffset.Now,
                Scores = newSnapshot
            };

            this.Logger.Debug($"Score Changed. Last: {lastSnapshotWithTimestamp}, New: {newSnapshotWithTimestamp}");
            this.LatestSnapshot = newSnapshotWithTimestamp;

            var args = new ScoreChangedEventArgs(
                latestSnapshot: lastSnapshotWithTimestamp,
                previousSnapshot: newSnapshotWithTimestamp);

            this.ScoreChanged?.Invoke(this, args);
            this.Logger.Debug($"{nameof(UpdateSnapshot)}() called.");
        }
        else
        {
            this.Logger.Debug($"No change in score. Last: {lastSnapshotWithTimestamp}, New: {newSnapshot}");
        }
    }

    public event EventHandler<ScoreChangedEventArgs>? ScoreChanged;
}

public record ScoreSnapshot
{
    public required int Score { get; init; }
    public required int MaxScore { get; init; }
    public required int ModifiedScore { get; init; }
    public required int MaxModifiedScore { get; init; }

    public required int Multiplier { get; init; } = 1;
    public required float MultiplierProgress { get; init; }
}

public record TimestampedScoreSnapshot
{
    public required DateTimeOffset Timestamp { get; init; }
    public required ScoreSnapshot Scores { get; init; }
}

//public class ScoreMonitor2 : MonitorBase
//{
//    public IScoreController ScoreController { get; }
//    public ScoreMultiplierCounter ScoreMultiplierCounter { get; }

//    public int CurrentScore => this.ScoreController.modifiedScore;
//    public int CurrentMaxScore => this.ScoreController.immediateMaxPossibleMultipliedScore;
//    public int CurrentMultiplier => this.ScoreMultiplierCounter.multiplier;
//    public float CurrentMultiplierNormalizedProgress => this.ScoreMultiplierCounter.normalizedProgress;

//    public ScoreMonitor2(
//        IScoreController scoreController,
//        ScoreMultiplierCounter scoreMultiplierCounter)
//    {
//        this.ScoreController = scoreController;
//        this.ScoreMultiplierCounter = scoreMultiplierCounter;

//        this.Register(
//            this.ScoreController,
//            sc => sc.scoringForNoteFinishedEvent += this.OnScoringForNoteFinished,
//            sc => sc.scoringForNoteFinishedEvent -= this.OnScoringForNoteFinished);

//        this.Register(
//            this.ScoreController,
//            sc => sc.multiplierDidChangeEvent += this.OnMultiplierDidChange,
//            sc => sc.multiplierDidChangeEvent -= this.OnMultiplierDidChange);
//    }

//    private float lastMultiplierProgress;
//    private int lastMultiplier;

//    private void OnScoringForNoteFinished(ScoringElement scoringElement)
//    {
//        this.ScoreChanged?.Invoke(this, new ScoreChangedEventArgs(this.CurrentScore, this.CurrentMaxScore));
//    }

//    private void OnMultiplierDidChange(int multiplier, float progress)
//    {
//        var exchangedMultiplier = Interlocked.CompareExchange(ref this.lastMultiplier, multiplier, this.lastMultiplier);
//        var exchangedProgress = Interlocked.CompareExchange(ref this.lastMultiplierProgress, progress, this.lastMultiplierProgress);

//        if (exchangedMultiplier != multiplier)
//        {
//            this.MultiplierChanged?.Invoke(this, new MultiplierChangedEventArgs(multiplier, progress));
//        }

//        if (exchangedProgress != progress)
//        {
//            this.MultiplierProgressChanged?.Invoke(this, new MultiplierProgressChangedEventArgs(progress));
//        }
//    }

//    public event EventHandler<ScoreChangedEventArgs>? ScoreChanged;
//    public event EventHandler<MultiplierChangedEventArgs>? MultiplierChanged;
//    public event EventHandler<MultiplierProgressChangedEventArgs>? MultiplierProgressChanged;
//}
