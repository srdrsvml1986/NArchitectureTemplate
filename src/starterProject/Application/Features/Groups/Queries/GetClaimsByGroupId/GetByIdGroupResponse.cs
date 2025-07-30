using Domain.DTos;
using Domain.Entities;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.Groups.Queries.GetClaimsByGroupId;

public class GetClaimsByGroupIdGroupResponse : IResponse
{
    public IList<OperationClaimDto>? Claims { get; set; }
}