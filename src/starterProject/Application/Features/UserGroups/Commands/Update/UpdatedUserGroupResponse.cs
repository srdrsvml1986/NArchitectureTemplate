using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.UserGroups.Commands.Update;

public class UpdatedUserGroupResponse : IResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GroupId { get; set; }
}