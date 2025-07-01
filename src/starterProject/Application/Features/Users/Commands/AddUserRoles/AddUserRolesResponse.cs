using Domain.Entities;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.Users.Commands.AddUserRoles;

public class AddUserRolesResponse : IResponse
{
    public IQueryable<Role>? Roles { get; set; }
}
