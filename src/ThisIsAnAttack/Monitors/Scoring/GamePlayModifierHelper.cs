using ThisIsAnAttack.Monitors.Scoring.Entities;

namespace ThisIsAnAttack.Monitors.Scoring;

public static class GamePlayModifierHelper
{
    /// <summary>
    /// Combines multiple GamePlayModifier flags into a single flag.
    /// </summary>
    /// <param name="modifiers">An array of flags to combine.</param>
    /// <returns>The combined flag.</returns>
    public static GamePlayModifier CombineModifiers(params GamePlayModifier[] modifiers)
    {
        var combined = GamePlayModifier.None;
        foreach (var modifier in modifiers)
        {
            combined |= modifier;
        }
        return combined;
    }
}
