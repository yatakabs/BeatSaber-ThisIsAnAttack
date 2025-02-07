namespace ThisIsAnAttack.Monitors.Scoring.Entities;

public record ScoreProgressDetails
{
    public required int NotesTotal { get; init; }
    public required int NotesHit { get; init; }
    public required int NotesMissed { get; init; }
    public required int NotesBadcut { get; init; }
    public required int BombsTotal { get; init; }
    public required int BombsHit { get; init; }
    public required int BombsPassed { get; init; }
    public required int ObstaclesTotal { get; init; }
    public required int ObstaclesHit { get; init; }
    public required int ObstaclesPassed { get; init; }
    public required int ObstaclesHitCount { get; init; }
    public required double ObstaclesHitDuration { get; init; }
    public required int MaxCombo { get; init; }
    public required int Combo { get; init; }
    public required int Multiplier { get; init; }
    public required double MultiplierProgress { get; init; }
    public required double Energy { get; init; }

    public static ScoreProgressDetails Empty { get; } = new ScoreProgressDetails
    {
        NotesTotal = 0,
        NotesHit = 0,
        NotesMissed = 0,
        NotesBadcut = 0,
        BombsTotal = 0,
        BombsHit = 0,
        BombsPassed = 0,
        ObstaclesTotal = 0,
        ObstaclesHit = 0,
        ObstaclesPassed = 0,
        ObstaclesHitCount = 0,
        ObstaclesHitDuration = 0,
        MaxCombo = 0,
        Combo = 0,
        Multiplier = 0,
        MultiplierProgress = 0,
        Energy = 0
    };
}
