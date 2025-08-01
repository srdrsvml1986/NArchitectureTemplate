using NArchitecture.Core.Application.Responses;

namespace Application.Features.Roles.Queries.GetById;

public class GetByIdRoleResponse : IResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}