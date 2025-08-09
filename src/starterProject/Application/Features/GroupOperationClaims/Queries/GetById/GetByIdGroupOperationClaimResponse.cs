using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.GroupOperationClaims.Queries.GetById;

public class GetByIdGroupOperationClaimResponse : IResponse
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public int GroupId { get; set; }
}