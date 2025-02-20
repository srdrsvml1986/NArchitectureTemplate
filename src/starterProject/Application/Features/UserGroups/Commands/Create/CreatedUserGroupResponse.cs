using NArchitecture.Core.Application.Responses;

namespace Application.Features.UserGroups.Commands.Create;

public class CreatedUserGroupResponse : IResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GroupId { get; set; }
}