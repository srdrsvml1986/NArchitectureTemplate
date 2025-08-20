namespace NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Services;
public interface IDatabaseLogger
{
    Task Info(string message, string? userId = null, string? service = null, string? action = null, object? details = null);
    Task Warn(string message, string? userId = null, string? service = null, string? action = null, object? details = null);
    Task Error(string message, string? userId = null, string? service = null, string? action = null, object? details = null);
}
