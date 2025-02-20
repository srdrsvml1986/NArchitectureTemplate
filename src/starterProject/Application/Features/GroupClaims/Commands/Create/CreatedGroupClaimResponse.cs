using NArchitecture.Core.Application.Responses;

namespace Application.Features.GroupClaims.Commands.Create;

public class CreatedGroupClaimResponse : IResponse
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public int GroupId { get; set; }
}