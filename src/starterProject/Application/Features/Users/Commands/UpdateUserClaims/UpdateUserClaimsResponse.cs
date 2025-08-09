using Domain.Entities;
using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.Users.Commands.UpdateUserClaims;

public class UpdateUserClaimsResponse : IResponse
{
    public IQueryable<OperationClaim>? Claims { get; set; }
}
