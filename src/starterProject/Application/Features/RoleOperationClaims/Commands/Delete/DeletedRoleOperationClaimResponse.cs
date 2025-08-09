using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.RoleOperationClaims.Commands.Delete;

public class DeletedRoleOperationClaimResponse : IResponse
{
    public int Id { get; set; }
}