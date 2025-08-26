using Application.Features.DeviceTokens.Rules;
using Application.Services.DeviceTokens;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.DeviceTokens.Commands.Create;

public class CreateDeviceTokenCommand : IRequest<CreatedDeviceTokenResponse>
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public string DeviceType { get; set; }
    public string DeviceName { get; set; }
    public class CreateDeviceTokenCommandHandler : IRequestHandler<CreateDeviceTokenCommand, CreatedDeviceTokenResponse>
    {
        private readonly IMapper _mapper;
        private readonly IDeviceTokenService _deviceTokenService;
        private readonly DeviceTokenBusinessRules _deviceTokenBusinessRules;

        public CreateDeviceTokenCommandHandler(IMapper mapper, IDeviceTokenService deviceTokenService,
                                         DeviceTokenBusinessRules deviceTokenBusinessRules)
        {
            _mapper = mapper;
            _deviceTokenService = deviceTokenService;
            _deviceTokenBusinessRules = deviceTokenBusinessRules;
        }

        public async Task<CreatedDeviceTokenResponse> Handle(CreateDeviceTokenCommand request, CancellationToken cancellationToken)
        {
            DeviceToken deviceToken = _mapper.Map<DeviceToken>(request);

            await _deviceTokenService.AddAsync(deviceToken);

            CreatedDeviceTokenResponse response = _mapper.Map<CreatedDeviceTokenResponse>(deviceToken);
            return response;
        }
    }
}