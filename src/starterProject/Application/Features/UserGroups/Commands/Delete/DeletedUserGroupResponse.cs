using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.UserGroups.Commands.Delete;

public class DeletedUserGroupResponse : IResponse
{
    public int Id { get; set; }
}