using NArchitecture.Core.Application.Responses;

namespace Application.Features.Claims.Queries.GetById;

public class GetByIdClaimResponse : IResponse
{
    public int Id { get; set; }
    public string Name { get; set; }

    public GetByIdClaimResponse()
    {
        Name = string.Empty;
    }

    public GetByIdClaimResponse(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
