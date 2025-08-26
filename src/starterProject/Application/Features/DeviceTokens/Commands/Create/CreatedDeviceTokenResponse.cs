using NArchitectureTemplate.Core.Application.Responses;
using Domain.Entities;

namespace Application.Features.DeviceTokens.Commands.Create;

public class CreatedDeviceTokenResponse : IResponse
{
    public Guid Id { get; set; }
}