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

namespace Application.Features.UserNotificationSettings.Commands.Create;

public class CreateUserNotificationSettingCommand : IRequest<CreatedUserNotificationSettingResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public required Guid UserId { get; set; }
    public required NotificationType NotificationType { get; set; }
    public required NotificationChannel NotificationChannel { get; set; }
    public required bool IsEnabled { get; set; }

    public string[] Roles => [Admin, Write, UserNotificationSettingsOperationClaims.Create];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetUserNotificationSettings"];

    public class CreateUserNotificationSettingCommandHandler : IRequestHandler<CreateUserNotificationSettingCommand, CreatedUserNotificationSettingResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserNotificationSettingService _userNotificationSettingService;
        private readonly UserNotificationSettingBusinessRules _userNotificationSettingBusinessRules;

        public CreateUserNotificationSettingCommandHandler(IMapper mapper, IUserNotificationSettingService userNotificationSettingService,
                                         UserNotificationSettingBusinessRules userNotificationSettingBusinessRules)
        {
            _mapper = mapper;
            _userNotificationSettingService = userNotificationSettingService;
            _userNotificationSettingBusinessRules = userNotificationSettingBusinessRules;
        }

        public async Task<CreatedUserNotificationSettingResponse> Handle(CreateUserNotificationSettingCommand request, CancellationToken cancellationToken)
        {
            UserNotificationSetting userNotificationSetting = _mapper.Map<UserNotificationSetting>(request);

            await _userNotificationSettingService.AddAsync(userNotificationSetting);

            CreatedUserNotificationSettingResponse response = _mapper.Map<CreatedUserNotificationSettingResponse>(userNotificationSetting);
            return response;
        }
    }
}