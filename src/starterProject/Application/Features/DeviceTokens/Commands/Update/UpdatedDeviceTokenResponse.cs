using NArchitectureTemplate.Core.Application.Responses;
using Domain.Entities;

namespace Application.Features.DeviceTokens.Commands.Update;

public class UpdatedDeviceTokenResponse : IResponse
{
    public Guid Id { get; set; }
}