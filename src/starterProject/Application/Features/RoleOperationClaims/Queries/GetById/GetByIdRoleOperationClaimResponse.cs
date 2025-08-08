using NArchitecture.Core.Application.Responses;

namespace Application.Features.RoleOperationClaims.Queries.GetById;

public class GetByIdRoleOperationClaimResponse : IResponse
{
    public int Id { get; set; }
    public int OperationClaimId { get; set; }
    public int RoleId { get; set; }
}