using NArchitecture.Core.Application.Responses;

namespace Application.Features.RoleClaims.Queries.GetById;

public class GetByIdRoleClaimResponse : IResponse
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public int RoleId { get; set; }
}