using System.Text.RegularExpressions;
using ThisIsAnAttack.Configuration;
using ThisIsAnAttack.Disposables;
using ThisIsAnAttack.Logging;
using ThisIsAnAttack.Monitors;
using ThisIsAnAttack.Monitors.Scoring;
using ThisIsAnAttack.Monitors.Scoring.Entities;
using ThisIsAnAttack.Remoting;
using Zenject;

namespace ThisIsAnAttack.Services;

public class GameScoreStreamingService : SingleRunningServiceBaseWithLogging
{
    public Guid GameSessionId { get; } = Guid.NewGuid();

    public IRealtimeScoreSubmitter RealtimeScoreSubmitter { get; }
    public PluginConfig PluginConfig { get; }
    public ScoreMonitor ScoreMonitor { get; }
    public ComboMonitor ComboMonitor { get; }
    public EnergyMonitor EnergyMonitor { get; }
    public PauseMonitor PauseMonitor { get; }

    public IAudioTimeSource AudioTimeSource { get; }
    public GameplayCoreSceneSetupData GameplayCoreSceneSetupData { get; }
    public GameplayModifiers GameplayModifiers { get; }
    public IReadonlyBeatmapData BeatmapData { get; }

    private string PlayerId { get; }

    public GameScoreStreamingService(
        IRealtimeScoreSubmitter realtimeScoreSubmitter,
        IPluginLogger logger,
        ScoreMonitor scoreMonitor,
        ComboMonitor comboMonitor,
        EnergyMonitor energyMonitor,
        PauseMonitor pauseMonitor,
        PluginConfig pluginConfig,
        IAudioTimeSource audioTimeSource,
        GameplayCoreSceneSetupData gameplayCoreSceneSetupData,
        GameplayModifiers gameplayModifiers,
        IReadonlyBeatmapData beatmapData)
        : base(logger)
    {
        this.RealtimeScoreSubmitter = realtimeScoreSubmitter;
        this.ScoreMonitor = scoreMonitor;
        this.ComboMonitor = comboMonitor;
        this.EnergyMonitor = energyMonitor;
        this.PauseMonitor = pauseMonitor;
        this.PluginConfig = pluginConfig;
        this.AudioTimeSource = audioTimeSource;
        this.GameplayCoreSceneSetupData = gameplayCoreSceneSetupData;
        this.GameplayModifiers = gameplayModifiers;
        this.BeatmapData = beatmapData;


        var scoreSaberId = this.PluginConfig.Player?.ScoreSaberId ?? string.Empty;
        this.PlayerId = string.IsNullOrWhiteSpace(scoreSaberId)
            ? this.GameSessionId.ToString()
            : scoreSaberId;
    }

    private volatile PlayerGameProgress? lastProgress;

    private PlayerGameProgress? GetLastScoreIfChanged()
    {
        var playerId = this.PlayerId;
        var now = DateTimeOffset.UtcNow;

        var score = this.ScoreMonitor.LatestSnapshot;
        var pause = this.PauseMonitor.LatestSnapshot;
        var combo = this.ComboMonitor.LatestSnapshot;
        var energy = this.EnergyMonitor.LatestSnapshot;

        var songTime = TimeSpan.FromSeconds(this.AudioTimeSource.songTime);
        var songLength = TimeSpan.FromSeconds(this.AudioTimeSource.songLength);

        var level = this.GameplayCoreSceneSetupData.difficultyBeatmap.level;
        var characteristic = this.GameplayCoreSceneSetupData.difficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic;
        var difficultyBeatmap = this.GameplayCoreSceneSetupData.difficultyBeatmap;

        static string? parseSongHashFromLevelId(string levelId)
        {
            const string RegexPattern = "^custom_level_[0-9A-F]{40}";
            const string WipSuffix = " WIP";

            var regex = new Regex(RegexPattern, RegexOptions.IgnoreCase);
            if (regex.IsMatch(levelId) && !levelId.EndsWith(WipSuffix))
            {
                return levelId.Substring(13, 40);
            }

            return null;
        }

        var songHash = parseSongHashFromLevelId(level.levelID) ?? string.Empty;

        var songMetadata = new SongMetadata
        {
            SongName = level.songName,
            SongSubName = level.songSubName,
            SongAuthorName = level.songAuthorName,
            LevelAuthorName = level.levelAuthorName,
            SongHash = level.levelID,
            BeatsPerMinute = level.beatsPerMinute,
            PreviewDuration = TimeSpan.FromSeconds(level.previewDuration),
            PreviewStartTime = TimeSpan.FromSeconds(level.previewStartTime),
        };

        var modifiers = new GamePlayModifier[]
        {
            this.GameplayModifiers.noFailOn0Energy ? GamePlayModifier.NoFail : GamePlayModifier.None,
            this.GameplayModifiers.instaFail ? GamePlayModifier.InstaFail : GamePlayModifier.None,
            this.GameplayModifiers.energyType == GameplayModifiers.EnergyType.Battery ? GamePlayModifier.BatteryEnergy : GamePlayModifier.None,
            this.GameplayModifiers.noBombs ? GamePlayModifier.NoBombs : GamePlayModifier.None,
            this.GameplayModifiers.enabledObstacleType == GameplayModifiers.EnabledObstacleType.NoObstacles ? GamePlayModifier.NoObstacles : GamePlayModifier.None,
            this.GameplayModifiers.noArrows ? GamePlayModifier.NoArrows : GamePlayModifier.None,
            this.GameplayModifiers.ghostNotes ? GamePlayModifier.GhostNotes : GamePlayModifier.None,
            this.GameplayModifiers.disappearingArrows ? GamePlayModifier.DisappearingArrows : GamePlayModifier.None,
            this.GameplayModifiers.smallCubes ? GamePlayModifier.SmallCubes : GamePlayModifier.None,
            this.GameplayModifiers.proMode ? GamePlayModifier.ProMode : GamePlayModifier.None,
            this.GameplayModifiers.strictAngles ? GamePlayModifier.StrictAngles : GamePlayModifier.None,
            this.GameplayModifiers.zenMode ? GamePlayModifier.ZenMode : GamePlayModifier.None,
            this.GameplayModifiers.songSpeed == GameplayModifiers.SongSpeed.Slower ? GamePlayModifier.SlowerSong : GamePlayModifier.None,
            this.GameplayModifiers.songSpeed == GameplayModifiers.SongSpeed.Faster ? GamePlayModifier.FasterSong : GamePlayModifier.None,
            this.GameplayModifiers.songSpeed == GameplayModifiers.SongSpeed.SuperFast ? GamePlayModifier.SuperFastSong : GamePlayModifier.None,
        }
        .Aggregate(GamePlayModifier.None, (acc, x) => acc | x);

        //this.BeatmapData.

        var isPractice = this.GameplayCoreSceneSetupData.practiceSettings != null;

        var gameMode = isPractice ? GameMode.Practice : GameMode.Solo;
        var beatMap = this.BeatmapData;

        var isSongTimeOver = songTime >= songLength;
        var isSoftFailed = energy.Energy <= 0 && modifiers.HasFlag(GamePlayModifier.NoFail);

        var gamePlayState = isSongTimeOver
            ? PlayerGamePlayStateFlags.Finished
            : pause.IsPaused
                ? PlayerGamePlayStateFlags.Paused
                : isSoftFailed
                    ? PlayerGamePlayStateFlags.SoftFailed
                    : PlayerGamePlayStateFlags.Playing;

        var last = this.lastProgress;

        var startedAt = last?.GameProgress.StartedAt;
        if (startedAt is null)
        {
            startedAt = this.GameplayCoreSceneSetupData.gameplayModifiers.songSpeedMul < 1
                    ? now.AddSeconds(-songTime.TotalSeconds / this.GameplayCoreSceneSetupData.gameplayModifiers.songSpeedMul)
                    : now.AddSeconds(-songTime.TotalSeconds);

            var totalPauseDuration = pause.TotalPauseDuration;
            if (totalPauseDuration > TimeSpan.Zero)
            {
                startedAt = startedAt.Value.Subtract(totalPauseDuration);
            }
        }

        if (pause.Current is not null)
        {
            var elapsed = now - pause.Current.StartedAt;
            pause = pause with
            {
                Current = pause.Current with
                {
                    Duration = elapsed,
                },

                TotalPauseDuration = pause.TotalPauseDuration + elapsed,
            };
        }

        var progress = (last ?? PlayerGameProgress.Empty) with
        {
            PlayerId = playerId,
            ClientTimestamp = now,
            SongTimestamp = songTime,

            GameMode = gameMode,
            GamePlayModifiers = modifiers,
            GamePlayState = gamePlayState,
            GamePlayStatistics = new GamePlayStatistics
            {
                GamePlayOptions = new GamePlayOptions
                {
                    GamePlayModifiers = modifiers,
                },

                PauseStatistics = new PauseStatistics
                {
                    IsCurrentlyPaused = pause.IsPaused,
                    TotalPauseCount = pause.PauseCount,
                    TotalPauseDuration = pause.TotalPauseDuration,
                    CurrentPauseDuration = pause.Current?.Duration,
                    CurrentPauseStartedAt = pause.Current?.StartedAt,
                },
            },
            GameProgress = new Monitors.Scoring.Entities.GameProgress
            {
                ScoreProgress = new ScoreProgress
                {
                    CurrentMaxScore = score.Scores.MaxScore,
                    CurrentMaxScoreModified = score.Scores.MaxModifiedScore,
                    CurrentScore = score.Scores.Score,
                    CurrentScoreModified = score.Scores.ModifiedScore,
                    Details = new ScoreProgressDetails
                    {
                        Multiplier = score.Scores.Multiplier,
                        MultiplierProgress = score.Scores.MultiplierProgress,
                        Combo = combo.Combo,
                        MaxCombo = combo.MaxCombo,
                        Energy = energy.Energy,

                        BombsTotal = beatMap.bombsCount,
                        ObstaclesTotal = beatMap.obstaclesCount,
                        NotesTotal = beatMap.cuttableNotesCount,

                        BombsHit = 0,
                        BombsPassed = 0,

                        NotesHit = 0,
                        NotesMissed = 0,
                        NotesBadcut = 0,

                        ObstaclesHit = 0,
                        ObstaclesHitCount = 0,
                        ObstaclesHitDuration = 0,
                        ObstaclesPassed = 0,
                    },
                },
                BeatmapLevelId = level.levelID,
                Characteristic = new Characteristic
                {
                    Name = characteristic.serializedName,
                    Label = characteristic.name,
                },
                Difficulty = new Difficulty
                {
                    Name = difficultyBeatmap.difficulty.ToString(),
                    Rank = difficultyBeatmap.difficultyRank,
                },
                SongHash = songHash,
                StartedAt = startedAt,
            },
            SongProgress = new SongProgress
            {
                Duration = songLength,
                Position = songTime,
            },
        };

        if (last is null)
        {
            this.Logger.Debug($"Past progress is null. Initial progress: {progress}");
            this.lastProgress = progress;
            return progress;
        }

        var lastProgressWithoutTimestamp = last with
        {
            ClientTimestamp = DateTimeOffset.MinValue,
            SongTimestamp = TimeSpan.Zero,

            GamePlayStatistics = last.GamePlayStatistics with
            {
                PauseStatistics = last.GamePlayStatistics.PauseStatistics with
                {
                    CurrentPauseDuration = TimeSpan.Zero,
                    TotalPauseDuration = TimeSpan.Zero,
                },
            },
        };

        var newScoreWithoutTimestamp = progress with
        {
            ClientTimestamp = DateTimeOffset.MinValue,
            SongTimestamp = TimeSpan.Zero,

            GamePlayStatistics = progress.GamePlayStatistics with
            {
                PauseStatistics = progress.GamePlayStatistics.PauseStatistics with
                {
                    CurrentPauseDuration = TimeSpan.Zero,
                    TotalPauseDuration = TimeSpan.Zero,
                },
            },
        };

        var isUpdated = lastProgressWithoutTimestamp != newScoreWithoutTimestamp;
        if (isUpdated)
        {
            this.Logger.Debug($"Progress updated. Last: {last}, New: {progress}");
            this.lastProgress = progress;
            return progress;
        }
        else if (progress.ClientTimestamp - last.ClientTimestamp > TimeSpan.FromSeconds(1))
        {
            this.Logger.Debug($"Progress not updated, but the max interval has passed. Last: {last}, New: {progress}");
            this.lastProgress = progress;
            return progress;
        }

        this.Logger.Debug($"Progress not updated. Last: {last}, New: {progress}");
        return null;
    }

    protected override async Task RunAsyncCore(
        CompositeDisposable disposables,
        CancellationToken stoppingToken)
    {
        this.Logger.Info($"{nameof(GameScoreStreamingService)} started. Stopping token: {stoppingToken.GetHashCode()}");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var latestProgress = this.GetLastScoreIfChanged();
                if (latestProgress is not null)
                {
                    var isSongFinished =
                        latestProgress.GamePlayState.HasFlag(PlayerGamePlayStateFlags.Failed)
                        || latestProgress.GamePlayState.HasFlag(PlayerGamePlayStateFlags.Finished)
                        || latestProgress.GamePlayState.HasFlag(PlayerGamePlayStateFlags.Quit);

                    if (isSongFinished)
                    {
                        this.Logger.Info($"Song is finished. Sending completion signal: {latestProgress}");
                        try
                        {
                            await this.RealtimeScoreSubmitter
                                .SubmitFinishScoreAsync(latestProgress)
                                .ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            this.Logger.Error(ex, "An error occurred while submitting the finish score.");
                        }

                        this.Logger.Info($"Song finished. Getting out of the score submission loop.");
                        break;
                    }
                    else
                    {
                        try
                        {
                            this.Logger.Debug($"Submitting a score: {latestProgress}");

                            await this.RealtimeScoreSubmitter
                                .SubmitScoreAsync(latestProgress)
                                .ConfigureAwait(false);

                            this.Logger.Debug($"Score submitted. {latestProgress}");
                        }
                        catch (Exception ex)
                        {
                            this.Logger.Error(ex, "An error occurred while submitting the score.");
                        }
                    }
                }

                await Task.Delay(250, stoppingToken).ConfigureAwait(false);
            }

            this.Logger.Info($"Out of score submission loop. Stopping token: {stoppingToken.GetHashCode()} ({stoppingToken.IsCancellationRequested})");

            // Wait for the stopping token to be triggered.
            this.Logger.Info($"Waiting for the stopping token to be triggered.");
            await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException ex) when (ex.CancellationToken == stoppingToken)
        {
            // Ignore the cancellation exception.
            this.Logger.Trace($"Score submission service has successfully stopped by the stopping token: {stoppingToken.GetHashCode()}");
        }

        this.Logger.Info($"{nameof(GameScoreStreamingService)} stopped. Stopping token: {stoppingToken.GetHashCode()}");
    }
}

public class ScoreSubmissionInitializer : IInitializable
{
    private IPluginLogger Logger { get; }

    private DiContainer Container { get; }

    private SceneChangeMonitor SceneChangeMonitor { get; }

    public ScoreSubmissionInitializer(
        IPluginLogger logger,
        DiContainer container,
        SceneChangeMonitor sceneChangeMonitor)
    {
        this.Logger = logger;
        this.Container = container;
        this.SceneChangeMonitor = sceneChangeMonitor;
    }

    public void Initialize()
    {
        this.Logger.Info($"{nameof(ScoreSubmissionInitializer)} initialized.");

        // Register scene bound actions for the GameCore scene.
        // The scene is loaded when the game starts and unloaded when the game ends.
        _ = this.SceneChangeMonitor
            .RegisterSceneBoundActions(
                scenePredicate: scene =>
                {
                    var isGameCore = scene.name == "GameCore";
                    this.Logger.Debug($"Scene predicate: {scene.name} -> {isGameCore}");
                    return isGameCore;
                },
                onStart: async scene =>
                {
                    this.Logger.Info("GameCore scene loaded.");

                    try
                    {
                        this.Logger.Debug($"Resolving the {nameof(GameScoreStreamingService)} service.");
                        var service = this.Container.Resolve<GameScoreStreamingService>();

                        this.Logger.Info($"Starting the {nameof(GameScoreStreamingService)} service. Session ID: {service.GameSessionId}");
                        await service.StartAsync(default).ConfigureAwait(false);
                        this.Logger.Info($"{nameof(GameScoreStreamingService)} service started. Session ID: {service.GameSessionId}");

                        return service;
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Error(
                            ex,
                            $"An error occurred while starting the {nameof(GameScoreStreamingService)} service.");
                    }

                    return null;
                },
                onComplete: async (scene, service) =>
                {
                    if (service == null)
                    {
                        this.Logger.Warn($"The {nameof(GameScoreStreamingService)} service was not started.");
                        return;
                    }

                    try
                    {
                        this.Logger.Info("GameCore scene unloaded.");

                        this.Logger.Info($"Stopping the {nameof(GameScoreStreamingService)} service. Session ID: {service.GameSessionId}");
                        await service.StopAsync().ConfigureAwait(false);

                        this.Logger.Info($"{nameof(GameScoreStreamingService)} service stopped. Session ID: {service.GameSessionId}");
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Error(ex, $"An error occurred while stopping the {nameof(GameScoreStreamingService)} service. Session ID: {service.GameSessionId}");
                    }
                });

        this.Logger.Info($"{nameof(ScoreSubmissionInitializer)} completed.");
    }
}
