namespace ThisIsAnAttack.Monitors.Scoring.Entities;

[Flags]
public enum GamePlayModifier
{
    None = 0,
    NoFail = 1 << 0,
    InstaFail = 1 << 1,
    BatteryEnergy = 1 << 2,
    NoBombs = 1 << 3,
    NoObstacles = 1 << 4,
    NoArrows = 1 << 5,
    GhostNotes = 1 << 6,
    DisappearingArrows = 1 << 7,
    SmallCubes = 1 << 8,
    ProMode = 1 << 9,
    StrictAngles = 1 << 10,
    ZenMode = 1 << 11,
    SlowerSong = 1 << 12,
    FasterSong = 1 << 13,
    SuperFastSong = 1 << 14
}
