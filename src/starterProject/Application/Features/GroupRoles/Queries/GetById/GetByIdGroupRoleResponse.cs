using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.GroupRoles.Queries.GetById;

public class GetByIdGroupRoleResponse : IResponse
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public int RoleId { get; set; }
}