using NArchitecture.Core.Persistence.Repositories;

namespace NArchitecture.Core.Security.Entities;

public class UserSecurityClaim<TId, TUserId, TClaimId> : Entity<TId>
{
    public TUserId UserId { get; set; }
    public TClaimId SecurityClaimId { get; set; }

    public UserSecurityClaim()
    {
        UserId = default!;
        SecurityClaimId = default!;
    }

    public UserSecurityClaim(TUserId userId, TClaimId operationClaimId)
    {
        UserId = userId;
        SecurityClaimId = operationClaimId;
    }

    public UserSecurityClaim(TId id, TUserId userId, TClaimId operationClaimId)
        : base(id)
    {
        UserId = userId;
        SecurityClaimId = operationClaimId;
    }
}
