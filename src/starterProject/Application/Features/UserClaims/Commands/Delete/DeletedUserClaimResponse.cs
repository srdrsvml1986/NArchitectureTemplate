using NArchitecture.Core.Application.Responses;

namespace Application.Features.UserClaims.Commands.Delete;

public class DeletedUserClaimResponse : IResponse
{
    public Guid Id { get; set; }
}
