using NArchitecture.Core.Application.Responses;

namespace Application.Features.OperationClaims.Commands.Update;

public class UpdateOperationClaimResponse : IResponse
{
    public int Id { get; set; }
    public string Name { get; set; }

    public UpdateOperationClaimResponse()
    {
        Name = string.Empty;
    }

    public UpdateOperationClaimResponse(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
