using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

/// <summary>
/// Kullanıcı oturumunu temsil eden varlık. 
/// Oturumun hangi kullanıcıya ait olduğu, 
/// oturumun açıldığı IP adresi, kullanıcı ajanı, 
/// giriş zamanı, oturumun iptal edilip edilmediği, 
/// şüpheli olup olmadığı ve konum bilgisi gibi detayları içerir.
/// </summary>
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
