using NArchitecture.Core.Application.Responses;

namespace Application.Features.UserSecurityClaims.Commands.Create;

public class CreatedUserSecurityClaimResponse : IResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int ClaimId { get; set; }
}
