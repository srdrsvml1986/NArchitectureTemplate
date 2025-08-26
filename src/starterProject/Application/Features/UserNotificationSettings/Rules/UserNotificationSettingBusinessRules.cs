using Application.Features.UserNotificationSettings.Constants;
using Application.Services.Repositories;
using NArchitectureTemplate.Core.Application.Rules;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.Types;
using NArchitectureTemplate.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.UserNotificationSettings.Rules;

public class UserNotificationSettingBusinessRules : BaseBusinessRules
{
    private readonly IUserNotificationSettingRepository _userNotificationSettingRepository;
    private readonly ILocalizationService _localizationService;

    public UserNotificationSettingBusinessRules(IUserNotificationSettingRepository userNotificationSettingRepository, ILocalizationService localizationService)
    {
        _userNotificationSettingRepository = userNotificationSettingRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, UserNotificationSettingsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task UserNotificationSettingShouldExistWhenSelected(UserNotificationSetting? userNotificationSetting)
    {
        if (userNotificationSetting == null)
            await throwBusinessException(UserNotificationSettingsBusinessMessages.UserNotificationSettingNotExists);
    }

    public async Task UserNotificationSettingIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken)
    {
        UserNotificationSetting? userNotificationSetting = await _userNotificationSettingRepository.GetAsync(
            predicate: uns => uns.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await UserNotificationSettingShouldExistWhenSelected(userNotificationSetting);
    }
}