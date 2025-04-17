using NArchitecture.Core.Application.Responses;

namespace Application.Features.SecurityClaims.Queries.GetById;

public class GetByIdSecurityClaimResponse : IResponse
{
    public int Id { get; set; }
    public string Name { get; set; }

    public GetByIdSecurityClaimResponse()
    {
        Name = string.Empty;
    }

    public GetByIdSecurityClaimResponse(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
