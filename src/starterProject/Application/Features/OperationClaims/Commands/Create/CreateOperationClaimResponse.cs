using NArchitecture.Core.Application.Responses;

namespace Application.Features.OperationClaims.Commands.Create;

public class CreateOperationClaimResponse : IResponse
{
    public int Id { get; set; }
    public string Name { get; set; }

    public CreateOperationClaimResponse()
    {
        Name = string.Empty;
    }

    public CreateOperationClaimResponse(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
