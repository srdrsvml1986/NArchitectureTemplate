using NArchitecture.Core.Application.Responses;

namespace Application.Features.GroupClaims.Queries.GetById;

public class GetByIdGroupClaimResponse : IResponse
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public int GroupId { get; set; }
}