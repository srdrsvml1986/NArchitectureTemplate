using NArchitecture.Core.Application.Responses;

namespace Application.Features.UserSecurityClaims.Commands.Update;

public class UpdatedUserSecurityClaimResponse : IResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int SecurityClaimId { get; set; }
}
