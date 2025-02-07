namespace ThisIsAnAttack.Monitors.Scoring.Entities;

[Flags]
public enum GameMode
{
    None = 0,
    Solo = 1,
    Party = 2,
    Campaign = 4,
    Multiplayer = 8,
    Practice = 16
}
