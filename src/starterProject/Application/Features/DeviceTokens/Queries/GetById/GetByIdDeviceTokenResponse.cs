using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.DeviceTokens.Queries.GetById;

public class GetByIdDeviceTokenResponse : IResponse
{
    public Guid Id { get; set; }
}