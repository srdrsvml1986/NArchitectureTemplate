using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

public class UserRole : Entity<int>
{
    public Guid UserId { get; set; }
    public int RoleId { get; set; }

    // Navigation Properties
    public virtual User User { get; set; }
    public virtual Role Role { get; set; }
}

