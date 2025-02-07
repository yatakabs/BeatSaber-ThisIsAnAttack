namespace ThisIsAnAttack.Monitors.Scoring.Entities;

public record PauseStatistics
{
    /// <summary>
    /// Indicates if the game is currently paused.
    /// </summary>
    public required bool IsCurrentlyPaused { get; init; }

    /// <summary>
    /// Unix timestamp when the current pause started.
    /// </summary>
    public required DateTimeOffset? CurrentPauseStartedAt { get; init; }

    /// <summary>
    /// Duration of the current pause in milliseconds.
    /// </summary>
    public required TimeSpan? CurrentPauseDuration { get; init; }

    /// <summary>
    /// Total duration of all pauses in milliseconds.
    /// </summary>
    public required TimeSpan TotalPauseDuration { get; init; }

    /// <summary>
    /// Total number of pauses that have occurred.
    /// </summary>
    public required long TotalPauseCount { get; init; }

    public static PauseStatistics Empty { get; } = new PauseStatistics
    {
        IsCurrentlyPaused = false,
        CurrentPauseStartedAt = null,
        CurrentPauseDuration = null,
        TotalPauseDuration = TimeSpan.Zero,
        TotalPauseCount = 0,
    };
}
