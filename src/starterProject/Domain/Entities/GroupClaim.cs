using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class GroupClaim : Entity<int>
{
    public int ClaimId { get; set; }
    public virtual Claim Claim { get; set; }

    public int GroupId { get; set; }
    public virtual Group Group { get; set; }
}