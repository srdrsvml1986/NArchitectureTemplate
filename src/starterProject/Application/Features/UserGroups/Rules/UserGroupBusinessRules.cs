using Application.Features.UserGroups.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.UserGroups.Rules;

public class UserGroupBusinessRules : BaseBusinessRules
{
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly ILocalizationService _localizationService;

    public UserGroupBusinessRules(IUserGroupRepository userGroupRepository, ILocalizationService localizationService)
    {
        _userGroupRepository = userGroupRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, UserGroupsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task UserGroupShouldExistWhenSelected(UserGroup? userGroup)
    {
        if (userGroup == null)
            await throwBusinessException(UserGroupsBusinessMessages.UserGroupNotExists);
    }

    public async Task UserGroupIdShouldExistWhenSelected(int id, CancellationToken cancellationToken)
    {
        UserGroup? userGroup = await _userGroupRepository.GetAsync(
            predicate: ug => ug.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await UserGroupShouldExistWhenSelected(userGroup);
    }
}