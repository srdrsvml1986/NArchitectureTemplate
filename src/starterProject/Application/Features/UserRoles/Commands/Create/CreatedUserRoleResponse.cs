using NArchitecture.Core.Application.Responses;
using Domain.Entities;

namespace Application.Features.UserRoles.Commands.Create;

public class CreatedUserRoleResponse : IResponse
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int RoleId { get; set; }
}