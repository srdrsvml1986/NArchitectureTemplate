using NArchitecture.Core.Persistence.Repositories;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;
public class ResetPasswordToken:Entity<int>
{
    public required string Token { get; set; }
    public DateTime ExpirationDate { get; set; }

    [ForeignKey(nameof(User))]
    public required Guid UserId { get; set; }
    public virtual User? User { get; set; }
}
