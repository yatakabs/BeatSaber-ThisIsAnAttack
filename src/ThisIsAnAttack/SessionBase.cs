using System.Diagnostics;

namespace ThisIsAnAttack;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record MapPlayProgress(
    int NoteCut,
    int NoteMissed,
    int NoteBadCut,
    int BombCut,
    int WallHit,
    TimeSpan TotalWallHitDuration,
    TimeSpan TotalPauseDuration,
    int Multiplier,
    float MultiplierProgress,
    int Combo,
    float Energy,
    int PoseCount)
{
    private string GetDebuggerDisplay()
    {
        return this.ToString();
    }

    public static MapPlayProgress Initial { get; } = new(
        NoteCut: 0,
        NoteMissed: 0,
        NoteBadCut: 0,
        BombCut: 0,
        WallHit: 0,
        TotalWallHitDuration: TimeSpan.Zero,
        TotalPauseDuration: TimeSpan.Zero,
        Multiplier: 1,
        MultiplierProgress: 0,
        Combo: 0,
        Energy: 0,
        PoseCount: 0);
}
