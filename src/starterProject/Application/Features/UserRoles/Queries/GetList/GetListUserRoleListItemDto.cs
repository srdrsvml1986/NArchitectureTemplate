using NArchitecture.Core.Application.Dtos;
using Domain.Entities;

namespace Application.Features.UserRoles.Queries.GetList;

public class GetListUserRoleListItemDto : IDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int RoleId { get; set; }
}