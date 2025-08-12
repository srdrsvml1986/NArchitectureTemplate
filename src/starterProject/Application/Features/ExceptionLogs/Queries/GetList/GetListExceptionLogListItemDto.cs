using NArchitectureTemplate.Core.Application.Dtos;
using Domain.Entities;

namespace Application.Features.ExceptionLogs.Queries.GetList;

public class GetListExceptionLogListItemDto : IDto
{
    public Guid Id { get; set; }
}