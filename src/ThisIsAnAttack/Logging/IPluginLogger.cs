using System.Runtime.CompilerServices;

namespace ThisIsAnAttack.Logging;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Naming", "CA1716:Identifiers should not match keywords",
    Justification = "This is for 'Error' methods and it's so common.")]
public interface IPluginLogger
{
    void Debug(
        string message,
        [CallerMemberName] string? callerMemberName = null);

    void Debug(
        Exception exception,
        [CallerMemberName] string? callerMemberName = null);

    void Error(
        string message,
        [CallerMemberName] string? callerMemberName = null);

    void Critical(
        string message,
        [CallerMemberName] string? callerMemberName = null);

    void Critical(
        Exception exception,
        [CallerMemberName] string? callerMemberName = null);

    void Info(
        string message,
        [CallerMemberName] string? callerMemberName = null);

    void Info(
        Exception exception,
        [CallerMemberName] string? callerMemberName = null);

    void Trace(
        string message,
        [CallerMemberName] string? callerMemberName = null);

    void Warn(
        string message,
        [CallerMemberName] string? callerMemberName = null);
    void Warn(
        Exception exception,
        [CallerMemberName] string? callerMemberName = null);

    void Log(
        LogLevel logLevel,
        Exception exception,
        string message,
        [CallerMemberName] string? callerMemberName = null);

    void Log(
        LogLevel logLevel,
        string message,
        [CallerMemberName] string? callerMemberName = null);
}
