using Application.Features.DeviceTokens.Constants;
using Application.Features.DeviceTokens.Rules;
using Application.Services.DeviceTokens;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.DeviceTokens.Commands.Delete;

public class DeleteDeviceTokenCommand : IRequest<DeletedDeviceTokenResponse>
{
    public Guid Id { get; set; }

    public class DeleteDeviceTokenCommandHandler : IRequestHandler<DeleteDeviceTokenCommand, DeletedDeviceTokenResponse>
    {
        private readonly IMapper _mapper;
        private readonly IDeviceTokenService _deviceTokenService;
        private readonly DeviceTokenBusinessRules _deviceTokenBusinessRules;

        public DeleteDeviceTokenCommandHandler(IMapper mapper, IDeviceTokenService deviceTokenService,
                                         DeviceTokenBusinessRules deviceTokenBusinessRules)
        {
            _mapper = mapper;
            _deviceTokenService = deviceTokenService;
            _deviceTokenBusinessRules = deviceTokenBusinessRules;
        }

        public async Task<DeletedDeviceTokenResponse> Handle(DeleteDeviceTokenCommand request, CancellationToken cancellationToken)
        {
            DeviceToken? deviceToken = await _deviceTokenService.GetAsync(predicate: dt => dt.Id == request.Id, cancellationToken: cancellationToken);
            await _deviceTokenBusinessRules.DeviceTokenShouldExistWhenSelected(deviceToken);

            await _deviceTokenService.DeleteAsync(deviceToken!);

            DeletedDeviceTokenResponse response = _mapper.Map<DeletedDeviceTokenResponse>(deviceToken);
            return response;
        }
    }
}