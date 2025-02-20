using NArchitecture.Core.Application.Dtos;

namespace Application.Features.UserGroups.Queries.GetList;

public class GetListUserGroupListItemDto : IDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GroupId { get; set; }
}