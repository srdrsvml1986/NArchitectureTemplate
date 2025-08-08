using NArchitecture.Core.Application.Responses;

namespace Application.Features.OperationClaims.Commands.Delete;

public class DeleteOperationClaimResponse : IResponse
{
    public int Id { get; set; }
}
