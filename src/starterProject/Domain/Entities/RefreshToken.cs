using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;
public class RefreshToken : NArchitectureTemplate.Core.Security.Entities.RefreshToken<Guid, Guid>
{
    public Guid UserSessionId { get; set; } // Yeni eklenen özellik

    public virtual User User { get; set; } = default!;
    [ForeignKey(nameof(UserSessionId))]
    public virtual UserSession UserSession { get; set; } = default!;
}
