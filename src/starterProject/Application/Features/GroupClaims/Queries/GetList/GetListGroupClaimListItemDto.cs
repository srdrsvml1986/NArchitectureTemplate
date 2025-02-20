using NArchitecture.Core.Application.Dtos;

namespace Application.Features.GroupClaims.Queries.GetList;

public class GetListGroupClaimListItemDto : IDto
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public int GroupId { get; set; }
}