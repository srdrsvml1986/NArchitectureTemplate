using NArchitecture.Core.Application.Responses;

namespace Application.Features.SecurityClaims.Commands.Delete;

public class DeleteSecurityClaimResponse : IResponse
{
    public int Id { get; set; }
}
