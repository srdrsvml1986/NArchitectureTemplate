using Application.Features.UserNotificationSettings.Constants;
using Application.Services.UserNotificationSettings;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Pipelines.Caching;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using NArchitectureTemplate.Core.Persistence.Paging;
using MediatR;
using static Application.Features.UserNotificationSettings.Constants.UserNotificationSettingsOperationClaims;

namespace Application.Features.UserNotificationSettings.Queries.GetList;

public class GetListUserNotificationSettingQuery : IRequest<GetListResponse<GetListUserNotificationSettingListItemDto>>, ISecuredRequest, ICachableRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"GetListUserNotificationSettings({PageRequest.PageIndex},{PageRequest.PageSize})";
    public string? CacheGroupKey => "GetUserNotificationSettings";
    public TimeSpan? SlidingExpiration { get; }

    public class GetListUserNotificationSettingQueryHandler : IRequestHandler<GetListUserNotificationSettingQuery, GetListResponse<GetListUserNotificationSettingListItemDto>>
    {
        private readonly IUserNotificationSettingService _userNotificationSettingService;
        private readonly IMapper _mapper;

        public GetListUserNotificationSettingQueryHandler(IUserNotificationSettingService userNotificationSettingService, IMapper mapper)
        {
            _userNotificationSettingService = userNotificationSettingService;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListUserNotificationSettingListItemDto>> Handle(GetListUserNotificationSettingQuery request, CancellationToken cancellationToken)
        {
            IPaginate<UserNotificationSetting> userNotificationSettings = await _userNotificationSettingService.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

                        GetListResponse<GetListUserNotificationSettingListItemDto> response;
            if (userNotificationSettings.Items == null || !userNotificationSettings.Items.Any())
            {
                response = new GetListResponse<GetListUserNotificationSettingListItemDto>
                {
                    Items = new List<GetListUserNotificationSettingListItemDto>(),
                    Index = userNotificationSettings.Index,
                    Size = userNotificationSettings.Size,
                    Count = userNotificationSettings.Count,
                    Pages = userNotificationSettings.Pages
                };
            }
            else
            {
                response = _mapper.Map<GetListResponse<GetListUserNotificationSettingListItemDto>>(userNotificationSettings);
            }

            return response;
        }
    }
}