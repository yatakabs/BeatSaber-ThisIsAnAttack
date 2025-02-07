namespace ThisIsAnAttack.Monitors.Scoring;

public class MultiplierProgressChangedEventArgs : EventArgs
{
    public float Multiplier { get; }
    public float Progress { get; }
    public MultiplierProgressChangedEventArgs(float progress)

    {
        this.Progress = progress;
    }
}
