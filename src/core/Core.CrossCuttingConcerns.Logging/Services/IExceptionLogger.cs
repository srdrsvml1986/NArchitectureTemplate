namespace NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Services;

// IExceptionLogger.cs
public interface IExceptionLogger
{
    Task LogException(Exception ex, string? userId = null, string? service = null, string? action = null, object? details = null);
}
