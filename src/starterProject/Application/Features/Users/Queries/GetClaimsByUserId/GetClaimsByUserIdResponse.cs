using Domain.Entities;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.Users.Queries.GetClaimsByUserId;

public class GetClaimsByUserIdResponse : IResponse
{
    public IQueryable<Claim>? Claims { get; set; }
}
