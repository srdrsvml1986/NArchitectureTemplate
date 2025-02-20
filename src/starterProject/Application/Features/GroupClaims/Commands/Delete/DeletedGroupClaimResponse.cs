using NArchitecture.Core.Application.Responses;

namespace Application.Features.GroupClaims.Commands.Delete;

public class DeletedGroupClaimResponse : IResponse
{
    public int Id { get; set; }
}