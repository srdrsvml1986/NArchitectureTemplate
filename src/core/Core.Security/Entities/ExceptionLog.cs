using NArchitectureTemplate.Core.Persistence.Repositories;

namespace NArchitectureTemplate.Core.Security.Entities;

public class ExceptionLog<TId, TUserId> : Entity<TId>
{
    public ExceptionLog()
    {
    }

    public ExceptionLog(string exceptionType, string stackTrace, string level, string message, DateTime timestamp, TUserId? userId, string? service, string? action, string? details)
    {
        ExceptionType = exceptionType;
        StackTrace = stackTrace;
        Level = level;
        Message = message;
        Timestamp = timestamp;
        UserId = userId;
        Service = service;
        Action = action;
        Details = details;
    }

    public string ExceptionType { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public TUserId? UserId { get; set; }
    public string? Service { get; set; }
    public string? Action { get; set; }
    public string? Details { get; set; }

    


}