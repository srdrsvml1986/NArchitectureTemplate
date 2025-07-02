using NArchitecture.Core.Application.Dtos;
using Domain.Entities;

namespace Application.Features.RoleOperationClaims.Queries.GetList;

public class GetListRoleOperationClaimListItemDto : IDto
{
    public int Id { get; set; }
    public int OperationClaimId { get; set; }
    public int RoleId { get; set; }
}