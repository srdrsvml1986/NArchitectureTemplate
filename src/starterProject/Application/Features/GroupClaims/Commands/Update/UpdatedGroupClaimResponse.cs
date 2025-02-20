using NArchitecture.Core.Application.Responses;

namespace Application.Features.GroupClaims.Commands.Update;

public class UpdatedGroupClaimResponse : IResponse
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public int GroupId { get; set; }
}