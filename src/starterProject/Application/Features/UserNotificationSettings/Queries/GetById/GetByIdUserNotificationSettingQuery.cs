using Application.Features.UserNotificationSettings.Constants;
using Application.Features.UserNotificationSettings.Rules;
using Application.Services.UserNotificationSettings;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.UserNotificationSettings.Constants.UserNotificationSettingsOperationClaims;

namespace Application.Features.UserNotificationSettings.Queries.GetById;

public class GetByIdUserNotificationSettingQuery : IRequest<GetByIdUserNotificationSettingResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdUserNotificationSettingQueryHandler : IRequestHandler<GetByIdUserNotificationSettingQuery, GetByIdUserNotificationSettingResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserNotificationSettingService _userNotificationSettingService;
        private readonly UserNotificationSettingBusinessRules _userNotificationSettingBusinessRules;

        public GetByIdUserNotificationSettingQueryHandler(IMapper mapper, IUserNotificationSettingService userNotificationSettingService, UserNotificationSettingBusinessRules userNotificationSettingBusinessRules)
        {
            _mapper = mapper;
            _userNotificationSettingService = userNotificationSettingService;
            _userNotificationSettingBusinessRules = userNotificationSettingBusinessRules;
        }

        public async Task<GetByIdUserNotificationSettingResponse> Handle(GetByIdUserNotificationSettingQuery request, CancellationToken cancellationToken)
        {
            UserNotificationSetting? userNotificationSetting = await _userNotificationSettingService.GetAsync(predicate: uns => uns.Id == request.Id, cancellationToken: cancellationToken);
            await _userNotificationSettingBusinessRules.UserNotificationSettingShouldExistWhenSelected(userNotificationSetting);

            GetByIdUserNotificationSettingResponse response = _mapper.Map<GetByIdUserNotificationSettingResponse>(userNotificationSetting);
            return response;
        }
    }
}