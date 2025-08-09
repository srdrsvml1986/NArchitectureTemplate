using NArchitectureTemplate.Core.Application.Responses;
using Domain.Entities;

namespace Application.Features.UserRoles.Commands.Update;

public class UpdatedUserRoleResponse : IResponse
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int RoleId { get; set; }
}