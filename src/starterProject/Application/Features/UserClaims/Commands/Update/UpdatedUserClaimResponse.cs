using NArchitecture.Core.Application.Responses;

namespace Application.Features.UserClaims.Commands.Update;

public class UpdatedUserClaimResponse : IResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int SecurityClaimId { get; set; }
}
