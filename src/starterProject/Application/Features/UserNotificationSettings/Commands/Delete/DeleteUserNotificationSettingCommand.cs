using Application.Features.UserNotificationSettings.Constants;
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

namespace Application.Features.UserNotificationSettings.Commands.Delete;

public class DeleteUserNotificationSettingCommand : IRequest<DeletedUserNotificationSettingResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Write, UserNotificationSettingsOperationClaims.Delete];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetUserNotificationSettings"];

    public class DeleteUserNotificationSettingCommandHandler : IRequestHandler<DeleteUserNotificationSettingCommand, DeletedUserNotificationSettingResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserNotificationSettingService _userNotificationSettingService;
        private readonly UserNotificationSettingBusinessRules _userNotificationSettingBusinessRules;

        public DeleteUserNotificationSettingCommandHandler(IMapper mapper, IUserNotificationSettingService userNotificationSettingService,
                                         UserNotificationSettingBusinessRules userNotificationSettingBusinessRules)
        {
            _mapper = mapper;
            _userNotificationSettingService = userNotificationSettingService;
            _userNotificationSettingBusinessRules = userNotificationSettingBusinessRules;
        }

        public async Task<DeletedUserNotificationSettingResponse> Handle(DeleteUserNotificationSettingCommand request, CancellationToken cancellationToken)
        {
            UserNotificationSetting? userNotificationSetting = await _userNotificationSettingService.GetAsync(predicate: uns => uns.Id == request.Id, cancellationToken: cancellationToken);
            await _userNotificationSettingBusinessRules.UserNotificationSettingShouldExistWhenSelected(userNotificationSetting);

            await _userNotificationSettingService.DeleteAsync(userNotificationSetting!);

            DeletedUserNotificationSettingResponse response = _mapper.Map<DeletedUserNotificationSettingResponse>(userNotificationSetting);
            return response;
        }
    }
}