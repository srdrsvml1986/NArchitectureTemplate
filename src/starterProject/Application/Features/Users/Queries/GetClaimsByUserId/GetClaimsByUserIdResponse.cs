using Domain.Entities;
using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.Users.Queries.GetClaimsByUserId;

public class GetClaimsByUserIdResponse : IResponse
{
    public IQueryable<OperationClaim>? Claims { get; set; }
}
