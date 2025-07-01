using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

public class UserSession : Entity<Guid>
{
    public Guid UserId { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime LoginTime { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; } = false;
    public bool IsSuspicious { get; set; } = false;
    public string? LocationInfo { get; set; } // Örn: "Istanbul, TR"

    // Navigation Property
    public virtual User User { get; set; }
}
