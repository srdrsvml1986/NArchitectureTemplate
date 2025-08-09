using NArchitectureTemplate.Core.Application.Dtos;

namespace Application.Features.GroupOperationClaims.Queries.GetList;

public class GetListGroupOperationClaimListItemDto : IDto
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public int GroupId { get; set; }
}