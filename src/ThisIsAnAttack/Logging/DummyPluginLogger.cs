using System.Runtime.CompilerServices;

namespace ThisIsAnAttack.Logging;

/// <summary>
/// A dummy implementation of the <see cref="IPluginLogger"/> interface.
/// This class provides no-op implementations of the logging methods.
/// </summary>
public class DummyPluginLogger : IPluginLogger
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DummyPluginLogger"/> class.
    /// </summary>
    public DummyPluginLogger()
    {
    }

    /// <summary>
    /// Logs a critical message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    public void Critical(string message, [CallerMemberName] string? callerMemberName = null)
    {
        // Dummy implementation
    }

    /// <summary>
    /// Logs a critical exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    public void Critical(Exception exception, [CallerMemberName] string? callerMemberName = null)
    {
        // Dummy implementation
    }

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    public void Debug(string message, [CallerMemberName] string? callerMemberName = null)
    {
        // Dummy implementation
    }

    /// <summary>
    /// Logs a debug exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    public void Debug(Exception exception, [CallerMemberName] string? callerMemberName = null)
    {
        // Dummy implementation
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    public void Error(string message, [CallerMemberName] string? callerMemberName = null)
    {
        // Dummy implementation
    }

    /// <summary>
    /// Logs an error exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    public static void Error(Exception exception, [CallerMemberName] string? callerMemberName = null)
    {
        // Dummy implementation
    }

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    public void Info(string message, [CallerMemberName] string? callerMemberName = null)
    {
        // Dummy implementation
    }

    /// <summary>
    /// Logs an informational exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    public void Info(Exception exception, [CallerMemberName] string? callerMemberName = null)
    {
        // Dummy implementation
    }

    /// <summary>
    /// Logs a message with a specified log level.
    /// </summary>
    /// <param name="logLevel">The log level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    public void Log(
        LogLevel logLevel,
        Exception? exception,
        string message,
        [CallerMemberName] string? callerMemberName = null)
    {
        // Dummy implementation
    }

    /// <summary>
    /// Logs a message with a specified log level.
    /// </summary>
    /// <param name="logLevel">The log level.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    public void Log(LogLevel logLevel, string message, [CallerMemberName] string? callerMemberName = null)
    {
        // Dummy implementation
    }

    /// <summary>
    /// Logs a trace message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    public void Trace(string message, [CallerMemberName] string? callerMemberName = null)
    {
        // Dummy implementation
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    public void Warn(string message, [CallerMemberName] string? callerMemberName = null)
    {
        // Dummy implementation
    }

    /// <summary>
    /// Logs a warning exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    public void Warn(Exception exception, [CallerMemberName] string? callerMemberName = null)
    {
        // Dummy implementation
    }
}
