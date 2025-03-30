using Domain.Entities;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.Groups.Commands.UpdateClaimsInGroup;

public class UpdateClaimsInGroupResponse : IResponse
{
    public IQueryable<Claim>? Claims { get; set; }
}
