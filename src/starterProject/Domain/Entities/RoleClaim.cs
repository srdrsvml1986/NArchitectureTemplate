using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

public class RoleClaim : Entity<int>
{
    public int ClaimId { get; set; }
    public int RoleId { get; set; }

    // Navigation Properties
    public virtual Claim Claim { get; set; }
    public virtual Role Role { get; set; }
}
