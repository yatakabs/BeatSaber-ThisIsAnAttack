namespace ThisIsAnAttack.Monitors.Scoring.Entities;

[Flags]
public enum PlayerGamePlayState
{
    None = 0,
    Playing = 1,
    Paused = 2,
    Finished = 4,
    Failed = 8,
    Quit = 16,
    SoftFailed = 32
}
