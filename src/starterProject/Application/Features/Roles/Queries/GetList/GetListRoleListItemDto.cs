using NArchitecture.Core.Application.Dtos;
using Domain.Entities;

namespace Application.Features.Roles.Queries.GetList;

public class GetListRoleListItemDto : IDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}