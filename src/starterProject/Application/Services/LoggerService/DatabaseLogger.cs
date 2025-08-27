// DatabaseLogger.cs
using Application.Services.Logs;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction;

namespace Application.Services.LoggerService;

public class DatabaseLogger : ILogger
{
    private readonly ILogService _logService;

    public DatabaseLogger(ILogService logService)
    {
        _logService = logService;
    }

    public void Trace(string message, params object[] args) => Log("Trace", message, null, args);
    public void Debug(string message, params object[] args) => Log("Debug", message, null, args);
    public void Information(string message, params object[] args) => Log("Information", message, null, args);
    public void Warning(string message, params object[] args) => Log("Warning", message, null, args);
    public void Error(string message, params object[] args) => Log("Error", message, null, args);
    public void Error(Exception exception, string message, params object[] args) => Log("Error", message, exception, args);
    public void Critical(string message, params object[] args) => Log("Critical", message, null, args);

    private async void Log(string level, string message, Exception exception = null, params object[] args)
    {
        try
        {
            var formattedMessage = args?.Length > 0 ? string.Format(message, args) : message;

            var log = new Domain.Entities.Log
            {
                Level = level,
                Message = formattedMessage,
                Timestamp = DateTime.UtcNow
            };
            await _logService.AddAsync(log);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database logging failed: {ex.Message}");
        }
    }
}