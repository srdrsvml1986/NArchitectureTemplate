using NArchitectureTemplate.Core.Persistence.Repositories;

namespace NArchitectureTemplate.Core.Security.Entities;

public class RoleOperationClaim<TId, TRoleId, TOperationClaimId> : Entity<TId>
{
    public TRoleId RoleId { get; set; }
    public TOperationClaimId OperationClaimId { get; set; }

    public RoleOperationClaim()
    {
        RoleId = default!;
        OperationClaimId = default!;
    }

    public RoleOperationClaim(TRoleId roleId, TOperationClaimId operationClaimId)
    {
        RoleId = roleId;
        OperationClaimId = operationClaimId;
    }

    public RoleOperationClaim(TId id, TRoleId roleId, TOperationClaimId operationClaimId)
        : base(id)
    {
        RoleId = roleId;
        OperationClaimId = operationClaimId;
    }
}