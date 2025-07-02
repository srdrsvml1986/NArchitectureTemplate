using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class GroupOperationClaim : Entity<int>
{
    public int OperationClaimId { get; set; }
    public virtual OperationClaim OperationClaim { get; set; }

    public int GroupId { get; set; }
    public virtual Group Group { get; set; }
}