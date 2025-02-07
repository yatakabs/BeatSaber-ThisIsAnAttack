namespace ThisIsAnAttack.Remoting.Grpc;

public static class GrcpClientDataModelExtensions
{
    public static GameScore.GameProgress ToGrpcSchema(
        this Monitors.Scoring.Entities.GameProgress gameProgress)
    {
        return new GameScore.GameProgress
        {
            SongHash = gameProgress.SongHash,
            BeatmapLevelId = gameProgress.BeatmapLevelId,
            Characteristic = new GameScore.Characteristic
            {
                Name = gameProgress.Characteristic.Name,
                Label = gameProgress.Characteristic.Label ?? string.Empty
            },
            StartedAt = gameProgress.StartedAt?.ToUnixTimeMilliseconds() ?? 0,
            EndedAt = gameProgress.EndedAt?.ToUnixTimeMilliseconds() ?? 0,
            ScoreProgress = new GameScore.ScoreProgress
            {
                CurrentScore = gameProgress.ScoreProgress.CurrentScore,
                CurrentMaxScore = gameProgress.ScoreProgress.CurrentMaxScore,
                CurrentScoreModified = gameProgress.ScoreProgress.CurrentScoreModified,
                CurrentMaxScoreModified = gameProgress.ScoreProgress.CurrentMaxScoreModified,
                Details = new GameScore.ScoreProgressDetails
                {
                    Combo = gameProgress.ScoreProgress.Details.Combo,
                    MaxCombo = gameProgress.ScoreProgress.Details.MaxCombo,
                    Multiplier = gameProgress.ScoreProgress.Details.Multiplier,
                    MultiplierProgress = gameProgress.ScoreProgress.Details.MultiplierProgress,
                    BombsHit = gameProgress.ScoreProgress.Details.BombsHit,
                    BombsPassed = gameProgress.ScoreProgress.Details.BombsPassed,
                    NotesHit = gameProgress.ScoreProgress.Details.NotesHit,
                    NotesMissed = gameProgress.ScoreProgress.Details.NotesMissed,
                    ObstaclesHit = gameProgress.ScoreProgress.Details.ObstaclesHit,
                    ObstaclesPassed = gameProgress.ScoreProgress.Details.ObstaclesPassed,
                    ObstaclesHitCount = gameProgress.ScoreProgress.Details.ObstaclesHitCount,
                    ObstaclesHitDuration = gameProgress.ScoreProgress.Details.ObstaclesHitDuration,
                    NotesTotal = gameProgress.ScoreProgress.Details.NotesTotal,
                    BombsTotal = gameProgress.ScoreProgress.Details.BombsTotal,
                    ObstaclesTotal = gameProgress.ScoreProgress.Details.ObstaclesTotal,
                    Energy = gameProgress.ScoreProgress.Details.Energy,
                    NotesBadcut = gameProgress.ScoreProgress.Details.NotesBadcut,
                }
            },
            Difficulty = new GameScore.Difficulty
            {
                Rank = gameProgress.Difficulty.Rank,
                Name = gameProgress.Difficulty.Name ?? string.Empty,
                CustomLabel = gameProgress.Difficulty.CustomLabel ?? string.Empty
            }
        };
    }

    public static GameScore.GamePlayModifier[] ToGrpcSchema(
        this IEnumerable<Monitors.Scoring.Entities.GamePlayModifier> gamePlayModifiers)
    {
        return gamePlayModifiers
            .Select(x => (GameScore.GamePlayModifier)x)
            .ToArray();
    }

    public static GameScore.PlayerGameProgress ToGrpcSchema(
        this Monitors.Scoring.Entities.PlayerGameProgress playerGameProgress)
    {
        var progress = new GameScore.PlayerGameProgress()
        {
            PlayerId = playerGameProgress.PlayerId,
            GameProgress = playerGameProgress.GameProgress.ToGrpcSchema(),
            GamePlayState = (GameScore.PlayerGamePlayState)playerGameProgress.GamePlayState,
            GameMode = (GameScore.GameMode)playerGameProgress.GameMode,
            ClientTimestamp = playerGameProgress.ClientTimestamp.ToUnixTimeMilliseconds(),
            SongTimestamp = playerGameProgress.SongTimestamp.TotalSeconds,
            GamePlayStatistics = playerGameProgress.GamePlayStatistics.ToGrpcSchema(),
            SongProgress = new GameScore.SongProgress
            {
                Duration = playerGameProgress.SongProgress.Duration.TotalSeconds,
                Position = playerGameProgress.SongProgress.Position.TotalSeconds
            },
        };

        var modifiersFlags = playerGameProgress.GamePlayModifiers;
        var modifiers = Enum.GetValues(typeof(Monitors.Scoring.Entities.GamePlayModifier))
            .Cast<Monitors.Scoring.Entities.GamePlayModifier>()
            .Select(x => modifiersFlags.HasFlag(x) ? (GameScore.GamePlayModifier)x : GameScore.GamePlayModifier.None)
            .ToArray();

        progress.GamePlayModifiers.AddRange(modifiers);

        return progress;
    }

    public static GameScore.GamePlayStatistics ToGrpcSchema(
        this Monitors.Scoring.Entities.GamePlayStatistics gamePlayStatistics)
    {
        return new GameScore.GamePlayStatistics
        {
            GamePlayOptions = gamePlayStatistics.GamePlayOptions.ToGrpcSchema(),
            PauseStatistics = gamePlayStatistics.PauseStatistics.ToGrpcSchema()
        };
    }

    public static GameScore.PauseStatistics ToGrpcSchema(
        this Monitors.Scoring.Entities.PauseStatistics pauseStatistics)
    {
        return new GameScore.PauseStatistics
        {
            IsCurrentlyPaused = pauseStatistics.IsCurrentlyPaused,
            TotalPauseDuration = (long)pauseStatistics.TotalPauseDuration.TotalMilliseconds,
            CurrentPauseDuration = (long)(pauseStatistics.CurrentPauseDuration?.TotalMilliseconds ?? 0),
            CurrentPauseStartedAt = pauseStatistics.CurrentPauseStartedAt?.ToUnixTimeMilliseconds() ?? 0,
            TotalPauseCount = pauseStatistics.TotalPauseCount
        };
    }

    public static GameScore.GamePlayOptions ToGrpcSchema(
        this Monitors.Scoring.Entities.GamePlayOptions gamePlayOptions)
    {
        var result = new GameScore.GamePlayOptions();

        var modifiersFlags = gamePlayOptions.GamePlayModifiers;
        var modifiers = Enum.GetValues(typeof(Monitors.Scoring.Entities.GamePlayModifier))
            .Cast<Monitors.Scoring.Entities.GamePlayModifier>()
            .Select(x => modifiersFlags.HasFlag(x) ? (GameScore.GamePlayModifier)x : GameScore.GamePlayModifier.None)
            .ToArray();

        result.GamePlayModifiers.AddRange(modifiers);

        return result;
    }
}
