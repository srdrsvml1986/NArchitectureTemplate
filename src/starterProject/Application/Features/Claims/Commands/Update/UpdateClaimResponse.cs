using NArchitecture.Core.Application.Responses;

namespace Application.Features.Claims.Commands.Update;

public class UpdateClaimResponse : IResponse
{
    public int Id { get; set; }
    public string Name { get; set; }

    public UpdateClaimResponse()
    {
        Name = string.Empty;
    }

    public UpdateClaimResponse(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
