namespace ThisIsAnAttack.Monitors.Scoring.Entities;

public record GamePlayStatistics
{
    /// <summary>
    /// Options for the gameplay.
    /// </summary>
    public required GamePlayOptions GamePlayOptions { get; init; }

    /// <summary>
    /// Statistics related to game pauses.
    /// </summary>
    public required PauseStatistics PauseStatistics { get; init; }

    public static GamePlayStatistics Empty { get; } = new GamePlayStatistics
    {
        GamePlayOptions = GamePlayOptions.Empty,
        PauseStatistics = PauseStatistics.Empty,
    };
}
