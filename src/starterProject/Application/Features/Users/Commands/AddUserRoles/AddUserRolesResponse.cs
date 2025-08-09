using Domain.Entities;
using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.Users.Commands.AddUserRoles;

public class AddUserRolesResponse : IResponse
{
    public IQueryable<Role>? Roles { get; set; }
}
