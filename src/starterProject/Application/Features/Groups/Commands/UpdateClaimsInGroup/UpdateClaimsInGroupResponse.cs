using Domain.DTos;
using Domain.Entities;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.Groups.Commands.UpdateClaimsInGroup;

public class UpdateClaimsInGroupResponse : IResponse
{
    public IList<OperationClaimDto> Claims { get; set; }
}
