using Domain.Entities;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.Users.Commands.AddUserClaims;

public class AddUserClaimsResponse : IResponse
{
    public IQueryable<OperationClaim>? Claims { get; set; }
}
