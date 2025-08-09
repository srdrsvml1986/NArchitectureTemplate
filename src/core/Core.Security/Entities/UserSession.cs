using NArchitectureTemplate.Core.Persistence.Repositories;

namespace NArchitectureTemplate.Core.Security.Entities;

/// <summary>
/// Kullanıcı oturumunu temsil eden varlık. 
/// Oturumun hangi kullanıcıya ait olduğu, 
/// oturumun açıldığı IP adresi, kullanıcı ajanı, 
/// giriş zamanı, oturumun iptal edilip edilmediği, 
/// şüpheli olup olmadığı ve konum bilgisi gibi detayları içerir.
/// </summary>
public class UserSession<TId, TUserId> : Entity<TId>
{
    public TUserId UserId { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime LoginTime { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; } = false;
    public bool IsSuspicious { get; set; } = false;
    public string? LocationInfo { get; set; } // Örn: "Istanbul, TR"

    public UserSession()
    {
        IpAddress = string.Empty;
        UserAgent = string.Empty;
        UserId = default!;
    }

    public UserSession(TUserId userId, string ipAddress, string userAgent)
    {
        UserId = userId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }

    public UserSession(TId id, TUserId userId, string ipAddress, string userAgent)
        : base(id)
    {
        UserId = userId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }
}