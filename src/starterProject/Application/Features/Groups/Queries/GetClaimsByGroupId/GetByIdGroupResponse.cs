using Domain.Entities;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.Groups.Queries.GetClaimsByGroupId;

public class GetClaimsByGroupIdGroupResponse : IResponse
{
    public IQueryable<SecurityClaim>? Claims { get; set; }
}