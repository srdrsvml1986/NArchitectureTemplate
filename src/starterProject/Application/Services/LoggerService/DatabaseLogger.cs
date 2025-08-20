using Application.Services.Logs;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction;

namespace Application.Services.LoggerService;

public class DatabaseLogger : ILogger
{
    private readonly ILogService _logService;

    // Constructor'ı ILogService interface'ini kullanacak şekilde değiştirin
    public DatabaseLogger(ILogService logService)
    {
        _logService = logService;
    }

    public void Trace(string message) => Log("Trace", message);
    public void Debug(string message) => Log("Debug", message);
    public void Information(string message) => Log("Information", message);
    public void Warning(string message) => Log("Warning", message);
    public void Error(string message) => Log("Error", message);
    public void Critical(string message) => Log("Critical", message);

    private async void Log(string level, string message)
    {
        try
        {
            var log = new Domain.Entities.Log
            {
                Level = level,
                Message = message,
                Timestamp = DateTime.UtcNow,
            };
            await _logService.AddAsync(log);
        }
        catch (Exception ex)
        {
            // Fallback logging
            Console.WriteLine($"Database logging failed: {ex.Message}");
        }
    }
}