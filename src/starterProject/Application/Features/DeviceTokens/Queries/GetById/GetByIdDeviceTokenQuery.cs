using Application.Features.DeviceTokens.Rules;
using Application.Services.DeviceTokens;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.DeviceTokens.Queries.GetById;

public class GetByIdDeviceTokenQuery : IRequest<GetByIdDeviceTokenResponse>
{
    public Guid Id { get; set; }

    public class GetByIdDeviceTokenQueryHandler : IRequestHandler<GetByIdDeviceTokenQuery, GetByIdDeviceTokenResponse>
    {
        private readonly IMapper _mapper;
        private readonly IDeviceTokenService _deviceTokenService;
        private readonly DeviceTokenBusinessRules _deviceTokenBusinessRules;

        public GetByIdDeviceTokenQueryHandler(IMapper mapper, IDeviceTokenService deviceTokenService, DeviceTokenBusinessRules deviceTokenBusinessRules)
        {
            _mapper = mapper;
            _deviceTokenService = deviceTokenService;
            _deviceTokenBusinessRules = deviceTokenBusinessRules;
        }

        public async Task<GetByIdDeviceTokenResponse> Handle(GetByIdDeviceTokenQuery request, CancellationToken cancellationToken)
        {
            DeviceToken? deviceToken = await _deviceTokenService.GetAsync(predicate: dt => dt.Id == request.Id, cancellationToken: cancellationToken);
            await _deviceTokenBusinessRules.DeviceTokenShouldExistWhenSelected(deviceToken);

            GetByIdDeviceTokenResponse response = _mapper.Map<GetByIdDeviceTokenResponse>(deviceToken);
            return response;
        }
    }
}