using NArchitectureTemplate.Core.Application.Dtos;
using Domain.Entities;

namespace Application.Features.GroupRoles.Queries.GetList;

public class GetListGroupRoleListItemDto : IDto
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public int RoleId { get; set; }
}