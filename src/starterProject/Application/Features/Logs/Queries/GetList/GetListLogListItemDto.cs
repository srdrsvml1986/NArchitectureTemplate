using NArchitectureTemplate.Core.Application.Dtos;
using Domain.Entities;

namespace Application.Features.Logs.Queries.GetList;

public class GetListLogListItemDto : IDto
{
    public Guid Id { get; set; }
}