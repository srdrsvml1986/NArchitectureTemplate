using NArchitecture.Core.Application.Dtos;
using Domain.Entities;

namespace Application.Features.RoleClaims.Queries.GetList;

public class GetListRoleClaimListItemDto : IDto
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public int RoleId { get; set; }
}