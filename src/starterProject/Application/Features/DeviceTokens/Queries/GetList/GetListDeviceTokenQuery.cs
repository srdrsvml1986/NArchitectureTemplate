using Application.Services.DeviceTokens;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using NArchitectureTemplate.Core.Persistence.Paging;
using MediatR;

namespace Application.Features.DeviceTokens.Queries.GetList;

public class GetListDeviceTokenQuery : IRequest<GetListResponse<GetListDeviceTokenListItemDto>>
{
    public PageRequest PageRequest { get; set; }

    public class GetListDeviceTokenQueryHandler : IRequestHandler<GetListDeviceTokenQuery, GetListResponse<GetListDeviceTokenListItemDto>>
    {
        private readonly IDeviceTokenService _deviceTokenService;
        private readonly IMapper _mapper;

        public GetListDeviceTokenQueryHandler(IDeviceTokenService deviceTokenService, IMapper mapper)
        {
            _deviceTokenService = deviceTokenService;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListDeviceTokenListItemDto>> Handle(GetListDeviceTokenQuery request, CancellationToken cancellationToken)
        {
            IPaginate<DeviceToken> deviceTokens = await _deviceTokenService.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

                        GetListResponse<GetListDeviceTokenListItemDto> response;
            if (deviceTokens.Items == null || !deviceTokens.Items.Any())
            {
                response = new GetListResponse<GetListDeviceTokenListItemDto>
                {
                    Items = new List<GetListDeviceTokenListItemDto>(),
                    Index = deviceTokens.Index,
                    Size = deviceTokens.Size,
                    Count = deviceTokens.Count,
                    Pages = deviceTokens.Pages
                };
            }
            else
            {
                response = _mapper.Map<GetListResponse<GetListDeviceTokenListItemDto>>(deviceTokens);
            }

            return response;
        }
    }
}