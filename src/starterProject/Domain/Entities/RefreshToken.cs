namespace Domain.Entities;

public class RefreshToken : NArchitecture.Core.Security.Entities.RefreshToken<Guid, Guid>
{
    public Guid SessionId { get; set; } // Yeni eklenen özellik

    public virtual User User { get; set; } = default!;
    public virtual UserSession UserSession { get; set; } = default!;
}
