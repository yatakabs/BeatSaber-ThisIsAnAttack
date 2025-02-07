namespace ThisIsAnAttack.Monitors.Scoring.Entities;

public record PlayerGameProgress
{
    public required string PlayerId { get; init; }
    public required GameProgress GameProgress { get; init; }
    public required PlayerGamePlayStateFlags GamePlayState { get; init; }
    public required GameMode GameMode { get; init; }
    public required GamePlayModifier GamePlayModifiers { get; init; }
    public required DateTimeOffset ClientTimestamp { get; init; }
    public required TimeSpan SongTimestamp { get; init; }
    public required GamePlayStatistics GamePlayStatistics { get; init; }
    public required SongProgress SongProgress { get; init; }

    public static PlayerGameProgress Empty { get; } = new PlayerGameProgress
    {
        PlayerId = string.Empty,
        GameProgress = GameProgress.Empty,
        GamePlayState = PlayerGamePlayStateFlags.None,
        GameMode = GameMode.None,
        GamePlayModifiers = GamePlayModifier.None,
        ClientTimestamp = DateTimeOffset.MinValue,
        SongTimestamp = TimeSpan.Zero,
        GamePlayStatistics = GamePlayStatistics.Empty,
        SongProgress = SongProgress.Empty,
    };
}
