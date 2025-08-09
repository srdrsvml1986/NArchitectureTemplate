using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.UserRoles.Queries.GetById;

public class GetByIdUserRoleResponse : IResponse
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int RoleId { get; set; }
}