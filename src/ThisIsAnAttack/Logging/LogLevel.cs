namespace ThisIsAnAttack.Logging;

public enum LogLevel
{
    None = 0,
    Debug = 1,
    Info = 1 << 1,
    Warn = 1 << 2,
    Error = 1 << 3,
    Critical = 1 << 4,
    Trace = 1 << 6,
}
