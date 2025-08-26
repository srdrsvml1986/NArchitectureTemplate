using NArchitectureTemplate.Core.Application.Dtos;
using Domain.Entities;

namespace Application.Features.DeviceTokens.Queries.GetList;

public class GetListDeviceTokenListItemDto : IDto
{
    public Guid Id { get; set; }
}