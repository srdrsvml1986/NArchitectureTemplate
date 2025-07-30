using Domain.DTos;
using Domain.Entities;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.Groups.Commands.AddClaimsToGroup;

public class AddClaimsToGroupResponse : IResponse
{
    public IList<OperationClaimDto> Claims { get; set; }
}
