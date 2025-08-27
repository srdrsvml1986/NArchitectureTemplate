// ConsoleFallbackLogger.cs
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction;

namespace Application.Services.LoggerService;
public class ConsoleFallbackLogger : ILogger
{
    public void Trace(string message, params object[] args) =>
        Console.WriteLine($"[TRACE] {FormatMessage(message, args)}");

    public void Debug(string message, params object[] args) =>
        Console.WriteLine($"[DEBUG] {FormatMessage(message, args)}");

    public void Information(string message, params object[] args) =>
        Console.WriteLine($"[INFO] {FormatMessage(message, args)}");

    public void Warning(string message, params object[] args) =>
        Console.WriteLine($"[WARN] {FormatMessage(message, args)}");

    public void Error(string message, params object[] args) =>
        Console.WriteLine($"[ERROR] {FormatMessage(message, args)}");

    public void Error(Exception exception, string message, params object[] args) =>
        Console.WriteLine($"[ERROR] {FormatMessage(message, args)} - Exception: {exception?.Message}");

    public void Critical(string message, params object[] args) =>
        Console.WriteLine($"[CRITICAL] {FormatMessage(message, args)}");

    private string FormatMessage(string message, object[] args) =>
        args?.Length > 0 ? string.Format(message, args) : message;
}