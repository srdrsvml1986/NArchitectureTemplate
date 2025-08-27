// CompositeLogger.cs
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Configurations;

namespace Application.Services.LoggerService;
public class CompositeLogger : ILogger
{
    private readonly IEnumerable<ILogger> _loggers;
    private readonly LoggingConfig _config;

    public CompositeLogger(IEnumerable<ILogger> loggers, LoggingConfig config)
    {
        _loggers = loggers;
        _config = config;
    }

    private bool IsEnabled(LogLevel level, string category)
    {
        var configuredLevel = GetLogLevel(category);
        return LogLevelHelper.ShouldLog(configuredLevel, level);
    }

    private LogLevel GetLogLevel(string category)
    {
        if (category != null && _config.Override != null && _config.Override.TryGetValue(category, out var levelStr))
        {
            if (Enum.TryParse<LogLevel>(levelStr, true, out var level))
            {
                return level;
            }
        }

        if (Enum.TryParse<LogLevel>(_config.DefaultLogLevel, true, out var defaultLevel))
        {
            return defaultLevel;
        }

        return LogLevel.Information;
    }

    public void Trace(string message, params object[] args) => Log(LogLevel.Trace, message, null, args);
    public void Debug(string message, params object[] args) => Log(LogLevel.Debug, message, null, args);
    public void Information(string message, params object[] args) => Log(LogLevel.Information, message, null, args);
    public void Warning(string message, params object[] args) => Log(LogLevel.Warning, message, null, args);
    public void Critical(string message, params object[] args) => Log(LogLevel.Critical, message, null, args);
    public void Error(string message, params object[] args) => Log(LogLevel.Error, message, null, args);
    public void Error(Exception exception, string message, params object[] args) => Log(LogLevel.Error, message, exception, args);

    private void Log(LogLevel level, string message, Exception exception = null, params object[] args)
    {
        if (!IsEnabled(level, null)) return;

        var formattedMessage = args?.Length > 0 ? string.Format(message, args) : message;

        foreach (var logger in _loggers)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    logger.Trace(formattedMessage);
                    break;
                case LogLevel.Debug:
                    logger.Debug(formattedMessage);
                    break;
                case LogLevel.Information:
                    logger.Information(formattedMessage);
                    break;
                case LogLevel.Warning:
                    logger.Warning(formattedMessage);
                    break;
                case LogLevel.Error:
                    if (exception != null)
                        logger.Error(exception, formattedMessage);
                    else
                        logger.Error(formattedMessage);
                    break;
                case LogLevel.Critical:
                    logger.Critical(formattedMessage);
                    break;
            }
        }
    }
}