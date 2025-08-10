using NArchitectureTemplate.Core.Persistence.Repositories;

namespace NArchitectureTemplate.Core.Security.Entities;

public class Log<TId> : Entity<TId>
{
    public string Level { get; set; } // INFO, WARN, ERROR, vb.
    public string Message { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; } // Kullanıcı ID (nullable)
    public string? Service { get; set; } // Logun geldiği servis/modül
    public string? Action { get; set; } // Yapılan işlem
    public string? Details { get; set; } // JSON formatında ek detaylar
}
