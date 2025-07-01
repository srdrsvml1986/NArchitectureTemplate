using NArchitecture.Core.Application.Responses;

namespace Application.Features.Claims.Commands.Create;

public class CreatedClaimResponse : IResponse
{
    public int Id { get; set; }
    public string Name { get; set; }

    public CreatedClaimResponse()
    {
        Name = string.Empty;
    }

    public CreatedClaimResponse(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
