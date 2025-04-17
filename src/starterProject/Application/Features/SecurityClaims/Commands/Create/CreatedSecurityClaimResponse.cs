using NArchitecture.Core.Application.Responses;

namespace Application.Features.SecurityClaims.Commands.Create;

public class CreatedSecurityClaimResponse : IResponse
{
    public int Id { get; set; }
    public string Name { get; set; }

    public CreatedSecurityClaimResponse()
    {
        Name = string.Empty;
    }

    public CreatedSecurityClaimResponse(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
