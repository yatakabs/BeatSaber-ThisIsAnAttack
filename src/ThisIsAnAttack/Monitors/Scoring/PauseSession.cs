namespace ThisIsAnAttack.Monitors.Scoring;

public record PauseSession
{
    public required PauseState State { get; init; }
    public required DateTimeOffset StartedAt { get; init; }
    public required TimeSpan Duration { get; init; }
}
