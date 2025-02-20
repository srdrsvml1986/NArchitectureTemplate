using NArchitecture.Core.Application.Responses;

namespace Application.Features.Claims.Commands.Delete;

public class DeletedClaimResponse : IResponse
{
    public int Id { get; set; }
}
