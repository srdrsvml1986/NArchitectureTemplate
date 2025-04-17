using NArchitecture.Core.Application.Responses;

namespace Application.Features.UserSecurityClaims.Commands.Delete;

public class DeletedUserSecurityClaimResponse : IResponse
{
    public Guid Id { get; set; }
}
