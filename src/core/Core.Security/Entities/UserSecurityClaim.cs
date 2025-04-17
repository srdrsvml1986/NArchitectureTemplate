using NArchitecture.Core.Persistence.Repositories;

namespace NArchitecture.Core.Security.Entities;

public class UserSecurityClaim<TId, TUserId, TClaimId> : Entity<TId>
{
    public TUserId UserId { get; set; }
    public TClaimId ClaimId { get; set; }

    public UserSecurityClaim()
    {
        UserId = default!;
        ClaimId = default!;
    }

    public UserSecurityClaim(TUserId userId, TClaimId operationClaimId)
    {
        UserId = userId;
        ClaimId = operationClaimId;
    }

    public UserSecurityClaim(TId id, TUserId userId, TClaimId operationClaimId)
        : base(id)
    {
        UserId = userId;
        ClaimId = operationClaimId;
    }
}
