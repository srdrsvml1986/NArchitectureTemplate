using Application.Features.UserNotificationSettings.Constants;
using Application.Features.UserNotificationSettings.Rules;
using Application.Services.UserNotificationSettings;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Pipelines.Caching;
using NArchitectureTemplate.Core.Application.Pipelines.Logging;
using NArchitectureTemplate.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.UserNotificationSettings.Constants.UserNotificationSettingsOperationClaims;

namespace Application.Features.UserNotificationSettings.Commands.Update;

public class UpdateUserNotificationSettingCommand : IRequest<UpdatedUserNotificationSettingResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required NotificationType NotificationType { get; set; }
    public required NotificationChannel NotificationChannel { get; set; }
    public required bool IsEnabled { get; set; }

    public string[] Roles => [Admin, Write, UserNotificationSettingsOperationClaims.Update];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetUserNotificationSettings"];

    public class UpdateUserNotificationSettingCommandHandler : IRequestHandler<UpdateUserNotificationSettingCommand, UpdatedUserNotificationSettingResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserNotificationSettingService _userNotificationSettingService;
        private readonly UserNotificationSettingBusinessRules _userNotificationSettingBusinessRules;

        public UpdateUserNotificationSettingCommandHandler(IMapper mapper, IUserNotificationSettingService userNotificationSettingService,
                                         UserNotificationSettingBusinessRules userNotificationSettingBusinessRules)
        {
            _mapper = mapper;
            _userNotificationSettingService = userNotificationSettingService;
            _userNotificationSettingBusinessRules = userNotificationSettingBusinessRules;
        }

        public async Task<UpdatedUserNotificationSettingResponse> Handle(UpdateUserNotificationSettingCommand request, CancellationToken cancellationToken)
        {
            UserNotificationSetting? userNotificationSetting = await _userNotificationSettingService.GetAsync(predicate: uns => uns.Id == request.Id, cancellationToken: cancellationToken);
            await _userNotificationSettingBusinessRules.UserNotificationSettingShouldExistWhenSelected(userNotificationSetting);
            userNotificationSetting = _mapper.Map(request, userNotificationSetting);

            await _userNotificationSettingService.UpdateAsync(userNotificationSetting!);

            UpdatedUserNotificationSettingResponse response = _mapper.Map<UpdatedUserNotificationSettingResponse>(userNotificationSetting);
            return response;
        }
    }
}