using Application.Features.DeviceTokens.Rules;
using Application.Services.DeviceTokens;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.DeviceTokens.Commands.Update;

public class UpdateDeviceTokenCommand : IRequest<UpdatedDeviceTokenResponse>
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public string DeviceType { get; set; }
    public string DeviceName { get; set; }
    public bool IsActive { get; set; }

    public class UpdateDeviceTokenCommandHandler : IRequestHandler<UpdateDeviceTokenCommand, UpdatedDeviceTokenResponse>
    {
        private readonly IMapper _mapper;
        private readonly IDeviceTokenService _deviceTokenService;
        private readonly DeviceTokenBusinessRules _deviceTokenBusinessRules;

        public UpdateDeviceTokenCommandHandler(IMapper mapper, IDeviceTokenService deviceTokenService,
                                         DeviceTokenBusinessRules deviceTokenBusinessRules)
        {
            _mapper = mapper;
            _deviceTokenService = deviceTokenService;
            _deviceTokenBusinessRules = deviceTokenBusinessRules;
        }

        public async Task<UpdatedDeviceTokenResponse> Handle(UpdateDeviceTokenCommand request, CancellationToken cancellationToken)
        {
            DeviceToken? deviceToken = await _deviceTokenService.GetAsync(dt => dt.Id == request.Id, cancellationToken: cancellationToken);
            await _deviceTokenBusinessRules.DeviceTokenShouldExistWhenSelected(deviceToken);

            deviceToken = _mapper.Map(request, deviceToken);

            await _deviceTokenService.UpdateAsync(deviceToken!);

            UpdatedDeviceTokenResponse response = _mapper.Map<UpdatedDeviceTokenResponse>(deviceToken);
            return response;
        }
    }
}