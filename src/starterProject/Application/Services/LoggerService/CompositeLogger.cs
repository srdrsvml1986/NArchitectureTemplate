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
        // Eğer category null ise veya Override dictionary'sinde yoksa DefaultLogLevel kullan
        if (category != null && _config.Override != null && _config.Override.TryGetValue(category, out var levelStr))
        {
            if (Enum.TryParse<LogLevel>(levelStr, true, out var level))
            {
                return level;
            }
        }

        // Varsayılan log seviyesini döndür
        if (Enum.TryParse<LogLevel>(_config.DefaultLogLevel, true, out var defaultLevel))
        {
            return defaultLevel;
        }

        // Varsayılan olarak Information seviyesini döndür
        return LogLevel.Information;
    }

    public void Trace(string message) => Log(LogLevel.Trace, message);
    public void Debug(string message) => Log(LogLevel.Debug, message);
    public void Information(string message) => Log(LogLevel.Information, message);
    public void Warning(string message) => Log(LogLevel.Warning, message);
    public void Error(string message) => Log(LogLevel.Error, message);
    public void Critical(string message) => Log(LogLevel.Critical, message);

    private void Log(LogLevel level, string message)
    {
        if (!IsEnabled(level, null)) return;

        foreach (var logger in _loggers)
        {
            switch (level)
            {
                case LogLevel.Trace: logger.Trace(message); break;
                case LogLevel.Debug: logger.Debug(message); break;
                case LogLevel.Information: logger.Information(message); break;
                case LogLevel.Warning: logger.Warning(message); break;
                case LogLevel.Error: logger.Error(message); break;
                case LogLevel.Critical: logger.Critical(message); break;
            }
        }
    }
}