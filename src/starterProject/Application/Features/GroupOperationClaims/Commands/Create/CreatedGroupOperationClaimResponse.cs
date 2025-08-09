using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.GroupOperationClaims.Commands.Create;

public class CreatedGroupOperationClaimResponse : IResponse
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public int GroupId { get; set; }
}