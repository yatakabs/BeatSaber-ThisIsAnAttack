namespace ThisIsAnAttack.Monitors.Scoring;

public class MultiplierChangedEventArgs : EventArgs
{
    public int Multiplier { get; }
    public float Progress { get; }
    public MultiplierChangedEventArgs(int multiplier, float progress)
    {
        this.Multiplier = multiplier;
        this.Progress = progress;
    }
}
