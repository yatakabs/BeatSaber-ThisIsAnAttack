namespace ThisIsAnAttack.Monitors.Scoring.Entities;

public record Characteristic
{
    public required string Name { get; init; }
    public string? Label { get; init; }

    public static Characteristic Empty { get; } = new Characteristic
    {
        Name = string.Empty,
    };
}
