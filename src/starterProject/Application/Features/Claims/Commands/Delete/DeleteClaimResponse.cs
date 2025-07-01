using NArchitecture.Core.Application.Responses;

namespace Application.Features.Claims.Commands.Delete;

public class DeleteClaimResponse : IResponse
{
    public int Id { get; set; }
}
