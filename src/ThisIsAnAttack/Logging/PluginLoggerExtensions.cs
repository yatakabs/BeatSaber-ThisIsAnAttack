using System.Runtime.CompilerServices;

namespace ThisIsAnAttack.Logging;

/// <summary>
/// Extension methods for <see cref="IPluginLogger"/>.
/// </summary>
public static class PluginLoggerExtensions
{
    #region Logging method with message and exception

    /// <summary>
    /// Logs a message with an exception.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="logLevel">The log level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="callerMemberName">The name of the calling member.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Debug(
        this IPluginLogger logger,
        Exception exception,
        string message,
        [CallerMemberName] string? callerMemberName = null)
    {
        logger.Log(
            logLevel: LogLevel.Debug,
            exception: exception,
            message: message,
            callerMemberName: callerMemberName);
    }

    /// <summary>
    /// Logs a message with an exception.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="logLevel">The log level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="callerMemberName">The name of the calling member.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Error(
        this IPluginLogger logger,
        Exception exception,
        string message,
        [CallerMemberName] string? callerMemberName = null)
    {
        logger.Log(
            logLevel: LogLevel.Error,
            exception: exception,
            message: message,
            callerMemberName: callerMemberName);
    }

    /// <summary>
    /// Logs a message with an exception.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="logLevel">The log level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="callerMemberName">The name of the calling member.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Critical(
        this IPluginLogger logger,
        Exception exception,
        string message,
        [CallerMemberName] string? callerMemberName = null)
    {
        logger.Log(
            logLevel: LogLevel.Critical,
            exception: exception,
            message: message,
            callerMemberName: callerMemberName);
    }

    /// <summary>
    /// Logs a message with an exception.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="logLevel">The log level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="callerMemberName">The name of the calling member.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Info(
        this IPluginLogger logger,
        Exception exception,
        string message,
        [CallerMemberName] string? callerMemberName = null)
    {
        logger.Log(
            logLevel: LogLevel.Info,
            exception: exception,
            message: message,
            callerMemberName: callerMemberName);
    }

    /// <summary>
    /// Logs a message with an exception.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="logLevel">The log level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="callerMemberName">The name of the calling member.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Trace(
        this IPluginLogger logger,
        Exception exception,
        string message,
        [CallerMemberName] string? callerMemberName = null)
    {
        logger.Log(
            logLevel: LogLevel.Trace,
            exception: exception,
            message: message,
            callerMemberName: callerMemberName);
    }

    /// <summary>
    /// Logs a message with an exception.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="logLevel">The log level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="callerMemberName">The name of the calling member.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Warn(
        this IPluginLogger logger,
        Exception exception,
        string message,
        [CallerMemberName] string? callerMemberName = null)
    {
        logger.Log(
            logLevel: LogLevel.Warn,
            exception: exception,
            message: message,
            callerMemberName: callerMemberName);
    }

    #endregion Logging method with message and exception

    #region Logging method with exception and formatted message

    /// <summary>
    /// Logs a formatted message with an exception.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="logLevel">The log level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="format">The format string for the message.</param>
    /// <param name="args">The arguments to format the message.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogFormat(
        this IPluginLogger logger,
        LogLevel logLevel,
        Exception exception,
        string format,
        params object[] args)
    {
        logger.Log(
            logLevel: logLevel,
            exception: exception,
            message: string.Format(format, args));
    }

    /// <summary>
    /// Logs a formatted message with an exception.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="format">The format string for the message.</param>
    /// <param name="args">The arguments to format the message.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DebugFormat(
        this IPluginLogger logger,
        Exception exception,
        string format,
        params object[] args)
    {
        logger.LogFormat(
            LogLevel.Debug,
            exception: exception,
            format: format,
            args: args);
    }

    /// <summary>
    /// Logs a formatted message with an exception.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="format">The format string for the message.</param>
    /// <param name="args">The arguments to format the message.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ErrorFormat(
        this IPluginLogger logger,
        Exception exception,
        string format,
        params object[] args)
    {
        logger.LogFormat(
            LogLevel.Error,
            exception: exception,
            format: format,
            args: args);
    }

    /// <summary>
    /// Logs a formatted message with an exception.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="format">The format string for the message.</param>
    /// <param name="args">The arguments to format the message.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CriticalFormat(
        this IPluginLogger logger,
        Exception exception,
        string format,
        params object[] args)
    {
        logger.LogFormat(
            LogLevel.Critical,
            exception: exception,
            format: format,
            args: args);
    }

    /// <summary>
    /// Logs a formatted message with an exception.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="format">The format string for the message.</param>
    /// <param name="args">The arguments to format the message.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InfoFormat(
        this IPluginLogger logger,
        Exception exception,
        string format,
        params object[] args)
    {
        logger.LogFormat(
            LogLevel.Info,
            exception: exception,
            format: format,
            args: args);
    }

    /// <summary>
    /// Logs a formatted message with an exception.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="format">The format string for the message.</param>
    /// <param name="args">The arguments to format the message.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void TraceFormat(
        this IPluginLogger logger,
        Exception exception,
        string format,
        params object[] args)
    {
        logger.LogFormat(
            LogLevel.Trace,
            exception: exception,
            format: format,
            args: args);
    }

    /// <summary>
    /// Logs a formatted message with an exception.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="format">The format string for the message.</param>
    /// <param name="args">The arguments to format the message.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WarnFormat(
        this IPluginLogger logger,
        Exception exception,
        string format,
        params object[] args)
    {
        logger.LogFormat(
            LogLevel.Warn,
            exception: exception,
            format: format,
            args: args);
    }

    #endregion Logging method with exception and formatted message

    #region Logging method with formatted message

    /// <summary>
    /// Logs a formatted message with a log level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="logLevel">The log level.</param>
    /// <param name="format">The format string for the message.</param>
    /// <param name="args">The arguments to format the message.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogFormat(
        this IPluginLogger logger,
        LogLevel logLevel,
        string format,
        params object[] args)
    {
        logger.Log(
            logLevel: logLevel,
            message: string.Format(format, args),
            callerMemberName: null);
    }

    /// <summary>
    /// Logs a formatted message with a log level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="format">The format string for the message.</param>
    /// <param name="args">The arguments to format the message.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DebugFormat(
        this IPluginLogger logger,
        string format,
        params object[] args)
    {
        logger.LogFormat(
            LogLevel.Debug,
            format: format,
            args: args);
    }

    /// <summary>
    /// Logs a formatted message with a log level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="format">The format string for the message.</param>
    /// <param name="args">The arguments to format the message.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ErrorFormat(
        this IPluginLogger logger,
        string format,
        params object[] args)
    {
        logger.LogFormat(
            LogLevel.Error,
            format: format,
            args: args);
    }

    /// <summary>
    /// Logs a formatted message with a log level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="format">The format string for the message.</param>
    /// <param name="args">The arguments to format the message.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CriticalFormat(
        this IPluginLogger logger,
        string format,
        params object[] args)
    {
        logger.LogFormat(
            LogLevel.Critical,
            format: format,
            args: args);
    }

    /// <summary>
    /// Logs a formatted message with a log level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="format">The format string for the message.</param>
    /// <param name="args">The arguments to format the message.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InfoFormat(
        this IPluginLogger logger,
        string format,
        params object[] args)
    {
        logger.LogFormat(
            LogLevel.Info,
            format: format,
            args: args);
    }

    /// <summary>
    /// Logs a formatted message with a log level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="format">The format string for the message.</param>
    /// <param name="args">The arguments to format the message.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void TraceFormat(
        this IPluginLogger logger,
        string format,
        params object[] args)
    {
        logger.LogFormat(
            LogLevel.Trace,
            format: format,
            args: args);
    }

    /// <summary>
    /// Logs a formatted message with a log level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="format">The format string for the message.</param>
    /// <param name="args">The arguments to format the message.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WarnFormat(
        this IPluginLogger logger,
        string format,
        params object[] args)
    {
        logger.LogFormat(
            LogLevel.Warn,
            format: format,
            args: args);
    }

    #endregion Logging method with formatted message
}
