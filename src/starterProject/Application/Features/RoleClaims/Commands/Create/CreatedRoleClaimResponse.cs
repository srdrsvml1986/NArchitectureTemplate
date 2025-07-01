using NArchitecture.Core.Application.Responses;
using Domain.Entities;

namespace Application.Features.RoleClaims.Commands.Create;

public class CreatedRoleClaimResponse : IResponse
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public int RoleId { get; set; }
}