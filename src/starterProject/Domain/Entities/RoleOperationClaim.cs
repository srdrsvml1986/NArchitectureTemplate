using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

public class RoleOperationClaim : Entity<int>
{
    public int OperationClaimId { get; set; }
    public int RoleId { get; set; }

    // Navigation Properties
    public virtual OperationClaim OperationClaim { get; set; }
    public virtual Role Role { get; set; }
}
