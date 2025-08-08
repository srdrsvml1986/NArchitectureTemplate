using NArchitecture.Core.Application.Responses;

namespace Application.Features.Roles.Commands.Delete;

public class DeletedRoleResponse : IResponse
{
    public int Id { get; set; }
}