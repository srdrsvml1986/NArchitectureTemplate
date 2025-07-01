using NArchitecture.Core.Application.Responses;
using Domain.Entities;

namespace Application.Features.GroupRoles.Commands.Update;

public class UpdatedGroupRoleResponse : IResponse
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public int RoleId { get; set; }
}