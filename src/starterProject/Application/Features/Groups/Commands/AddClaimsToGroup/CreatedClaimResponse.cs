using Domain.Entities;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.Groups.Commands.AddClaimsToGroup;

public class AddClaimsToGroupResponse : IResponse
{
    public IQueryable<OperationClaim>? Claims { get; set; }
}
