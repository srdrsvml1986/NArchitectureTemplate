using Domain.Entities;
using System.Collections.ObjectModel;

/// <summary>
/// Kullanıcı oturumunu temsil eden varlık. 
/// Oturumun hangi kullanıcıya ait olduğu, 
/// oturumun açıldığı IP adresi, kullanıcı ajanı, 
/// giriş zamanı, oturumun iptal edilip edilmediği, 
/// şüpheli olup olmadığı ve konum bilgisi gibi detayları içerir.
/// </summary>
public class UserSession : NArchitecture.Core.Security.Entities.UserSession<Guid,Guid>
{
    public virtual User User { get; set; }
    public virtual Collection<RefreshToken> RefreshTokens { get; set; }
}
