using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.GroupRoles.Commands.Delete;

public class DeletedGroupRoleResponse : IResponse
{
    public int Id { get; set; }
}