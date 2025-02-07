namespace ThisIsAnAttack.Monitors.Scoring.Entities;

public record Difficulty
{
    public required int Rank { get; init; }
    public required string Name { get; init; }
    public string? CustomLabel { get; init; }

    public static Difficulty Empty { get; } = new Difficulty
    {
        Rank = 0,
        Name = string.Empty,
    };
}
