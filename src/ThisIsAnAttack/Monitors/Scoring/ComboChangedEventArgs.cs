namespace ThisIsAnAttack.Monitors.Scoring;

public class ComboChangedEventArgs : EventArgs
{
    public ComboSnapshot LatestSnapshot { get; }
    public ComboSnapshot PreviousSnapshot { get; }

    public ComboChangedEventArgs(ComboSnapshot latestSnapshot, ComboSnapshot previousSnapshot)
    {
        this.LatestSnapshot = latestSnapshot;
        this.PreviousSnapshot = previousSnapshot;
    }
}

