namespace ThisIsAnAttack.Monitors.Scoring.Entities;

public record GameProgress
{
    public required string SongHash { get; init; } = string.Empty;
    public required string BeatmapLevelId { get; init; } = string.Empty;
    public required Characteristic Characteristic { get; init; }
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? EndedAt { get; init; }
    public required ScoreProgress ScoreProgress { get; init; }
    public required Difficulty Difficulty { get; init; }

    public static GameProgress Empty { get; } = new GameProgress
    {
        SongHash = string.Empty,
        BeatmapLevelId = string.Empty,
        Characteristic = Characteristic.Empty,
        ScoreProgress = ScoreProgress.Empty,
        Difficulty = Difficulty.Empty,
    };
}
