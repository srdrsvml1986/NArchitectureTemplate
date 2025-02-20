using NArchitecture.Core.Persistence.Repositories;

namespace NArchitecture.Core.Security.Entities;

public class UserClaim<TId, TUserId, TClaimId> : Entity<TId>
{
    public TUserId UserId { get; set; }
    public TClaimId ClaimId { get; set; }

    public UserClaim()
    {
        UserId = default!;
        ClaimId = default!;
    }

    public UserClaim(TUserId userId, TClaimId operationClaimId)
    {
        UserId = userId;
        ClaimId = operationClaimId;
    }

    public UserClaim(TId id, TUserId userId, TClaimId operationClaimId)
        : base(id)
    {
        UserId = userId;
        ClaimId = operationClaimId;
    }
}
