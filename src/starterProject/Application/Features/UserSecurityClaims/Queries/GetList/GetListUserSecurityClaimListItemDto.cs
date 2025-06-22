using NArchitecture.Core.Application.Dtos;

namespace Application.Features.UserClaims.Queries.GetList;

public class GetListUserClaimListItemDto : IDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int SecurityClaimId { get; set; }
}
