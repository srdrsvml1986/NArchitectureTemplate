using NArchitectureTemplate.Core.Persistence.Repositories;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;
public class ResetPasswordToken: NArchitectureTemplate.Core.Security.Entities.ResetPasswordToken<int,Guid>
{
    [ForeignKey(nameof(User))]
    public required Guid UserId { get; set; }
    public virtual User User { get; set; }
}
