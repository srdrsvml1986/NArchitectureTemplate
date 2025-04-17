using NArchitecture.Core.Application.Dtos;

namespace Application.Features.SecurityClaims.Queries.GetList;

public class GetListSecurityClaimListItemDto : IDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public GetListSecurityClaimListItemDto()
    {
        Name = string.Empty;
    }

    public GetListSecurityClaimListItemDto(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
