using NArchitecture.Core.Application.Responses;

namespace Application.Features.UserClaims.Commands.Create;

public class CreatedUserClaimResponse : IResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int ClaimId { get; set; }
}
