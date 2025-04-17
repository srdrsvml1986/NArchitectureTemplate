using NArchitecture.Core.Application.Responses;

namespace Application.Features.UserSecurityClaims.Queries.GetById;

public class GetByIdUserSecurityClaimResponse : IResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int ClaimId { get; set; }
}
