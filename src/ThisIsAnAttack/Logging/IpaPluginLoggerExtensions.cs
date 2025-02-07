namespace ThisIsAnAttack.Logging;

public static class IpaPluginLoggerExtensions
{
    #region LogLevel Conversion

    public static LogLevel ToPluginLogLevel(this IPA.Logging.Logger.Level logLevel)
    {
        return logLevel switch
        {
            IPA.Logging.Logger.Level.None => LogLevel.None,
            IPA.Logging.Logger.Level.Trace => LogLevel.Trace,
            IPA.Logging.Logger.Level.Debug => LogLevel.Debug,
            IPA.Logging.Logger.Level.Info => LogLevel.Info,
            IPA.Logging.Logger.Level.Warning => LogLevel.Warn,
            IPA.Logging.Logger.Level.Error => LogLevel.Error,
            IPA.Logging.Logger.Level.Critical => LogLevel.Critical,
            _ => throw new NotSupportedException($"The specified {nameof(IPA.Logging.Logger.Level)} is not supported: {logLevel}"),
        };
    }

    public static IPA.Logging.Logger.Level ToIpaLogLevel(this LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.None => IPA.Logging.Logger.Level.None,
            LogLevel.Trace => IPA.Logging.Logger.Level.Trace,
            LogLevel.Debug => IPA.Logging.Logger.Level.Debug,
            LogLevel.Info => IPA.Logging.Logger.Level.Info,
            LogLevel.Warn => IPA.Logging.Logger.Level.Warning,
            LogLevel.Error => IPA.Logging.Logger.Level.Error,
            LogLevel.Critical => IPA.Logging.Logger.Level.Critical,
            _ => throw new NotSupportedException($"The specified {nameof(LogLevel)} is not supported: {logLevel}"),
        };
    }

    #endregion LogLevel Conversion
}
