namespace ThisIsAnAttack.Monitors.Scoring;

public record GameProgress(
    SongProgress SongProgress,
    ImmidiateScore Score,
    ImmidiateStatistics Statistics);

public record ImmidiatePauseStatistics(
    bool IsPaused,
    int PauseCount,
    TimeSpan TotalPauseDuration);

public record ImmidiateWallStatistics(
    bool IsWallHit,
    int WallHitCount,
    TimeSpan TotalWallHitDuration);

public record ImmidiateScore(
    int MultipliedScore,
    int PossibleMultipliedMaxScore);

public record ImmidiateStatistics(
    int Combo,
    int MaxCombo,

    int BombsPassed,
    int BombsHit,

    int NotesPassed,
    int NotesHit,
    int NotesMissed,
    int NotesBadCut,

    ImmidiateWallStatistics WallStatistics,
    ImmidiatePauseStatistics PauseStatistics);
