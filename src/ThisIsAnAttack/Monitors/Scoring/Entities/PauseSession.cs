namespace ThisIsAnAttack.Monitors.Scoring.Entities;

public class PauseSession
{
    public long StartedAt { get; set; }
    public long EndedAt { get; set; }

    public long Duration => this.EndedAt - this.StartedAt;

    public TimeSpan DurationTimeSpan => TimeSpan.FromMilliseconds(this.Duration);
}
