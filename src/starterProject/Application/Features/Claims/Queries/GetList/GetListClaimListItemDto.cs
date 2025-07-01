using NArchitecture.Core.Application.Dtos;

namespace Application.Features.Claims.Queries.GetList;

public class GetListClaimListItemDto : IDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public GetListClaimListItemDto()
    {
        Name = string.Empty;
    }

    public GetListClaimListItemDto(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
