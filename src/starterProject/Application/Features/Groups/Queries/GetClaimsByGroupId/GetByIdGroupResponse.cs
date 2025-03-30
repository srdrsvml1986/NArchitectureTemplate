using Domain.Entities;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.Groups.Queries.GetClaimsByGroupId;

public class GetClaimsByGroupIdGroupResponse : IResponse
{
    public IQueryable<Claim>? Claims { get; set; }
}