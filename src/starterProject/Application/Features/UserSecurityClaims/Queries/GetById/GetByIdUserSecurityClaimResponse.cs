using NArchitecture.Core.Application.Responses;

namespace Application.Features.UserClaims.Queries.GetById;

public class GetByIdUserClaimResponse : IResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int SecurityClaimId { get; set; }
}
