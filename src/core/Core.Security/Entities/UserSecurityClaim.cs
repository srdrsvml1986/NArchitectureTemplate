using NArchitecture.Core.Persistence.Repositories;

namespace NArchitecture.Core.Security.Entities;

public class UserClaim<TId, TUserId, TClaimId> : Entity<TId>
{
    public TUserId UserId { get; set; }
    public TClaimId SecurityClaimId { get; set; }

    public UserClaim()
    {
        UserId = default!;
        SecurityClaimId = default!;
    }

    public UserClaim(TUserId userId, TClaimId operationClaimId)
    {
        UserId = userId;
        SecurityClaimId = operationClaimId;
    }

    public UserClaim(TId id, TUserId userId, TClaimId operationClaimId)
        : base(id)
    {
        UserId = userId;
        SecurityClaimId = operationClaimId;
    }
}
