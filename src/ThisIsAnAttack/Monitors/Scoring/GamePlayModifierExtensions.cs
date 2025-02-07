using ThisIsAnAttack.Monitors.Scoring.Entities;

namespace ThisIsAnAttack.Monitors.Scoring;

/// <summary>
/// Provides extension methods for converting between Protobuf GamePlayModifier and C# GamePlayModifierFlags.
/// </summary>
public static class GamePlayModifierExtensions
{
    /// <summary>
    /// Converts a Protobuf GamePlayModifier to a C# GamePlayModifierFlags.
    /// </summary>
    /// <param name="modifier">The Protobuf GamePlayModifier to convert.</param>
    /// <returns>The corresponding C# GamePlayModifierFlags.</returns>
    public static GamePlayModifier ToFlags(this GameScore.GamePlayModifier modifier)
    {
        return (GamePlayModifier)(int)modifier;
    }

    /// <summary>
    /// Converts a C# GamePlayModifierFlags to a Protobuf GamePlayModifier.
    /// </summary>
    /// <param name="flags">The C# GamePlayModifierFlags to convert.</param>
    /// <returns>The corresponding Protobuf GamePlayModifier.</returns>
    public static GameScore.GamePlayModifier ToProtobuf(this GamePlayModifier flags)
    {
        return (GameScore.GamePlayModifier)(int)flags;
    }
}
