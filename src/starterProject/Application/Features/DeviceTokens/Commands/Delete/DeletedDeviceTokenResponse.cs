using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.DeviceTokens.Commands.Delete;

public class DeletedDeviceTokenResponse : IResponse
{
    public Guid Id { get; set; }
}