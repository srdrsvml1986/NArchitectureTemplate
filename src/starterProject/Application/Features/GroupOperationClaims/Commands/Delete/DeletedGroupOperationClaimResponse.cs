using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.GroupOperationClaims.Commands.Delete;

public class DeletedGroupOperationClaimResponse : IResponse
{
    public int Id { get; set; }
}