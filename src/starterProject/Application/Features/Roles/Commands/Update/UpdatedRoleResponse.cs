using NArchitecture.Core.Application.Responses;
using Domain.Entities;

namespace Application.Features.Roles.Commands.Update;

public class UpdatedRoleResponse : IResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}