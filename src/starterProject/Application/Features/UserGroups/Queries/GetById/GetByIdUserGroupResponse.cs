using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.UserGroups.Queries.GetById;

public class GetByIdUserGroupResponse : IResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GroupId { get; set; }
}