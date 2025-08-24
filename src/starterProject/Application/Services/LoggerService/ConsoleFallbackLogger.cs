using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction;

namespace Application.Services.LoggerService;
public class ConsoleFallbackLogger : ILogger
{
    public void Trace(string message) => Console.WriteLine($"[TRACE] {message}");
    public void Debug(string message) => Console.WriteLine($"[DEBUG] {message}");
    public void Information(string message) => Console.WriteLine($"[INFO] {message}");
    public void Warning(string message) => Console.WriteLine($"[WARN] {message}");
    public void Error(Exception exception,string message) => Console.WriteLine($"[ERROR] {message}");
    public void Critical(string message) => Console.WriteLine($"[CRITICAL] {message}");

}