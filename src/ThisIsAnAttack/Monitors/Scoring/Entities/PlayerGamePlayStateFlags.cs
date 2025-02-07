namespace ThisIsAnAttack.Monitors.Scoring.Entities;

[Flags]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "<Pending>")]
public enum PlayerGamePlayStateFlags
{
    None = 0,
    Playing = 1 << 0,
    Paused = 1 << 1,
    Finished = 1 << 2,
    Failed = 1 << 3,
    Quit = 1 << 4,
    SoftFailed = 1 << 5
}
