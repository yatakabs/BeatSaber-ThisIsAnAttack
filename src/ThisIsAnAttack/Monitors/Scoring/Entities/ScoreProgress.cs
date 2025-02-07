namespace ThisIsAnAttack.Monitors.Scoring.Entities;

public record ScoreProgress
{
    public long CurrentScore { get; init; }
    public long CurrentMaxScore { get; init; }
    public long CurrentScoreModified { get; init; }
    public long CurrentMaxScoreModified { get; init; }
    public required ScoreProgressDetails Details { get; init; }

    public static ScoreProgress Empty { get; } = new ScoreProgress
    {
        CurrentScore = 0,
        CurrentMaxScore = 0,
        CurrentScoreModified = 0,
        CurrentMaxScoreModified = 0,
        Details = ScoreProgressDetails.Empty,
    };
}
