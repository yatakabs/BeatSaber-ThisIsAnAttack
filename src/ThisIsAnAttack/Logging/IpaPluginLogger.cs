using System.Runtime.CompilerServices;
using Zenject;
using IPALogger = IPA.Logging.Logger;

namespace ThisIsAnAttack.Logging;

public class IpaPluginLogger : PluginLoggerBase
{
    public delegate string LogMessageFormatter(
        string message,
        string callerMemberName);

    private IPALogger Logger { get; }
    private LogMessageFormatter MessageFormatter { get; }

    public static LogMessageFormatter DefaultMessageFormatter { get; } = (message, callerMemberName) =>
    {
        return string.IsNullOrWhiteSpace(message)
            ? string.Empty
            : callerMemberName != null
                ? $"[{callerMemberName}] {message}"
                : message;
    };

    [Inject]
    public IpaPluginLogger(IPALogger logger)
        : this(logger, DefaultMessageFormatter)
    {
    }

    public IpaPluginLogger(
        IPALogger logger,
        LogMessageFormatter? messageFormatter = null)
    {
        this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.MessageFormatter = messageFormatter ?? DefaultMessageFormatter;
    }

    public override void Log(
        LogLevel logLevel,
        Exception? exception,
        string message,
        [CallerMemberName] string? callerMemberName = null)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            var logMessage = this.MessageFormatter(message, callerMemberName ?? string.Empty) ?? message;

            this.Logger.Log(
                logLevel.ToIpaLogLevel(),
                logMessage);
        }

        if (exception != null)
        {
            this.Logger.Log(
                logLevel.ToIpaLogLevel(),
                exception);
        }
    }
}
