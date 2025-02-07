namespace ThisIsAnAttack.Monitors.Scoring;

public record ComboSnapshot
{
    public required DateTimeOffset DateTimeOffset { get; init; }
    public required int MaxCombo { get; init; }
    public required int Combo { get; init; }

    public static ComboSnapshot Empty { get; } = new ComboSnapshot
    {
        DateTimeOffset = DateTimeOffset.Now,
        MaxCombo = 0,
        Combo = 0,
    };

    public ComboSnapshot WithoutTimestamp()
    {
        return this with
        {
            DateTimeOffset = DateTimeOffset.MinValue,
        };
    }
}

