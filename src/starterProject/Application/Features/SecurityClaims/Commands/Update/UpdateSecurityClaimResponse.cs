using NArchitecture.Core.Application.Responses;

namespace Application.Features.SecurityClaims.Commands.Update;

public class UpdateSecurityClaimResponse : IResponse
{
    public int Id { get; set; }
    public string Name { get; set; }

    public UpdateSecurityClaimResponse()
    {
        Name = string.Empty;
    }

    public UpdateSecurityClaimResponse(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
