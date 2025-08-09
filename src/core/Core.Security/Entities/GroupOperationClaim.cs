using NArchitectureTemplate.Core.Persistence.Repositories;

namespace NArchitectureTemplate.Core.Security.Entities;

public class GroupOperationClaim<TId, TGroupId, TOperationClaimId> : Entity<TId>
{
    public TGroupId GroupId { get; set; }
    public TOperationClaimId OperationClaimId { get; set; }

    public GroupOperationClaim()
    {
        GroupId = default!;
        OperationClaimId = default!;
    }

    public GroupOperationClaim(TGroupId groupId, TOperationClaimId operationClaimId)
    {
        GroupId = groupId;
        OperationClaimId = operationClaimId;
    }

    public GroupOperationClaim(TId id, TGroupId groupId, TOperationClaimId operationClaimId)
        : base(id)
    {
        GroupId = groupId;
        OperationClaimId = operationClaimId;
    }
}