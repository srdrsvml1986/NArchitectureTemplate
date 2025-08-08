using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

/// <summary>
/// Kullanıcı oturumunu temsil eden varlık. 
/// Oturumun hangi kullanıcıya ait olduğu, 
/// oturumun açıldığı IP adresi, kullanıcı ajanı, 
/// giriş zamanı, oturumun iptal edilip edilmediği, 
/// şüpheli olup olmadığı ve konum bilgisi gibi detayları içerir.
/// </summary>
public class UserSession : NArchitecture.Core.Security.Entities.UserSession<Guid,Guid>
{
    // Navigation Property
    public virtual User User { get; set; }
}
