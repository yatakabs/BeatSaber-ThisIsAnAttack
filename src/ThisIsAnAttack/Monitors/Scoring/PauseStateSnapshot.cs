namespace ThisIsAnAttack.Monitors.Scoring;

public record PauseStateSnapshot
{
    public required DateTimeOffset Timestamp { get; init; }
    public required bool IsPaused { get; init; }
    public required int PauseCount { get; init; }
    public required TimeSpan TotalPauseDuration { get; init; }

    public required PauseSession? Current { get; init; }
    public required PauseSession[] History { get; init; }

    public static PauseStateSnapshot Empty { get; } = new PauseStateSnapshot
    {
        Timestamp = DateTimeOffset.MinValue,
        IsPaused = false,
        PauseCount = 0,
        TotalPauseDuration = TimeSpan.Zero,
        Current = null,
        History = [],
    };
    public PauseStateSnapshot WithoutTimestamp()
    {
        return this with
        {
            Timestamp = DateTimeOffset.MinValue,
        };
    }
}
