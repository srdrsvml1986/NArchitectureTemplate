using NArchitectureTemplate.Core.Application.Responses;
using Domain.Entities;

namespace Application.Features.RoleOperationClaims.Commands.Create;

public class CreatedRoleOperationClaimResponse : IResponse
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public int RoleId { get; set; }
}