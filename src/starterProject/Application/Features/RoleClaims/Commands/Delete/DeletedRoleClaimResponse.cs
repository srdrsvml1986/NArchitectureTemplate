using NArchitecture.Core.Application.Responses;

namespace Application.Features.RoleClaims.Commands.Delete;

public class DeletedRoleClaimResponse : IResponse
{
    public int Id { get; set; }
}