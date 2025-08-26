using Application.Features.DeviceTokens.Constants;
using Application.Services.Repositories;
using NArchitectureTemplate.Core.Application.Rules;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.Types;
using NArchitectureTemplate.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.DeviceTokens.Rules;

public class DeviceTokenBusinessRules : BaseBusinessRules
{
    private readonly IDeviceTokenRepository _deviceTokenRepository;
    private readonly ILocalizationService _localizationService;

    public DeviceTokenBusinessRules(IDeviceTokenRepository deviceTokenRepository, ILocalizationService localizationService)
    {
        _deviceTokenRepository = deviceTokenRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, DeviceTokensBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task DeviceTokenShouldExistWhenSelected(DeviceToken? deviceToken)
    {
        if (deviceToken == null)
            await throwBusinessException(DeviceTokensBusinessMessages.DeviceTokenNotExists);
    }

    public async Task DeviceTokenIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken)
    {
        DeviceToken? deviceToken = await _deviceTokenRepository.GetAsync(
            predicate: dt => dt.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await DeviceTokenShouldExistWhenSelected(deviceToken);
    }
}