namespace ThisIsAnAttack.Monitors.Scoring.Entities;

public record GamePlayOptions
{
    public required GamePlayModifier GamePlayModifiers { get; init; }

    public static GamePlayOptions Empty { get; } = new GamePlayOptions
    {
        GamePlayModifiers = GamePlayModifier.None,
    };
}
