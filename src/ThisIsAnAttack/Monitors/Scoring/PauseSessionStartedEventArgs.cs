namespace ThisIsAnAttack.Monitors.Scoring;

public class PauseSessionStartedEventArgs : EventArgs
{
    public PauseSession Session { get; }

    public PauseSessionStartedEventArgs(PauseSession session)
    {
        this.Session = session;
    }
}
