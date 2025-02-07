using ThisIsAnAttack.Monitors.Scoring;

namespace ThisIsAnAttack.Monitors;

public class ScoreChangedEventArgs : EventArgs
{
    public TimestampedScoreSnapshot LatestSnapshot { get; }
    public TimestampedScoreSnapshot PreviousSnapshot { get; }

    public ScoreChangedEventArgs(TimestampedScoreSnapshot latestSnapshot, TimestampedScoreSnapshot previousSnapshot)
    {
        this.LatestSnapshot = latestSnapshot;
        this.PreviousSnapshot = previousSnapshot;
    }
}
