using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

public class UserRole : Entity<int>
{
    public Guid UserId { get; set; }
    public int RoleId { get; set; }
    public DateTime AssignedAt { get; set; }
    public Guid AssignedBy { get; set; }

    // Navigation Properties
    public virtual User User { get; set; }
    public virtual Role Role { get; set; }
    public virtual User AssignedByUser { get; set; }
}
