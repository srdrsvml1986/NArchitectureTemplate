using Application.Features.UserSessions.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.UserSessions.Rules;

public class UserSessionBusinessRules : BaseBusinessRules
{
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly ILocalizationService _localizationService;

    public UserSessionBusinessRules(IUserSessionRepository userSessionRepository, ILocalizationService localizationService)
    {
        _userSessionRepository = userSessionRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, UserSessionsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task UserSessionShouldExistWhenSelected(UserSession? userSession)
    {
        if (userSession == null)
            await throwBusinessException(UserSessionsBusinessMessages.UserSessionNotExists);
    }

    public async Task UserSessionIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken)
    {
        UserSession? userSession = await _userSessionRepository.GetAsync(
            predicate: us => us.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await UserSessionShouldExistWhenSelected(userSession);
    }
}