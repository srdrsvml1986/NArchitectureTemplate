using NArchitecture.Core.Application.Responses;

namespace Application.Features.Claims.Commands.Update;

public class UpdatedClaimResponse : IResponse
{
    public int Id { get; set; }
    public string Name { get; set; }

    public UpdatedClaimResponse()
    {
        Name = string.Empty;
    }

    public UpdatedClaimResponse(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
