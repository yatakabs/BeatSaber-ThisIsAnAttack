namespace ThisIsAnAttack.Monitors.Scoring;

public class PauseSessionEndedEventArgs : EventArgs
{
    public PauseSession Session { get; }
    public TimeSpan TotalPauseDuration { get; }
    public int PauseCount { get; }

    public PauseSessionEndedEventArgs(PauseSession session, TimeSpan totalPauseDuration, int pauseCount)
    {
        this.Session = session;
        this.TotalPauseDuration = totalPauseDuration;
        this.PauseCount = pauseCount;
    }
}
