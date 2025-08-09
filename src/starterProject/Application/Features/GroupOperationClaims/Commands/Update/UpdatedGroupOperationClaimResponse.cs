using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.GroupOperationClaims.Commands.Update;

public class UpdatedGroupOperationClaimResponse : IResponse
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public int GroupId { get; set; }
}