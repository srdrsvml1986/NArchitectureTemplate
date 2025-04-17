using NArchitecture.Core.Application.Dtos;

namespace Application.Features.UserSecurityClaims.Queries.GetList;

public class GetListUserSecurityClaimListItemDto : IDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int OperationClaimId { get; set; }
}
