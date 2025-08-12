using NArchitectureTemplate.Core.Persistence.Repositories;

namespace NArchitectureTemplate.Core.Security.Entities;

public class Log<TId, TUserId> : Entity<TId>
{
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public TUserId? UserId { get; set; }
    public string? Service { get; set; }
    public string? Action { get; set; }
    public string? Details { get; set; }

    public Log() { }

    public Log(
        string level,
        string message,
        TUserId? userId = default,
        string? service = null,
        string? action = null,
        string? details = null)
    {
        Level = level;
        Message = message;
        UserId = userId;
        Service = service;
        Action = action;
        Details = details;
    }
}
