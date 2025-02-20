using Application.Features.Groups.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.Groups.Rules;

public class GroupBusinessRules : BaseBusinessRules
{
    private readonly IGroupRepository _groupRepository;
    private readonly ILocalizationService _localizationService;

    public GroupBusinessRules(IGroupRepository groupRepository, ILocalizationService localizationService)
    {
        _groupRepository = groupRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, GroupsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task GroupShouldExistWhenSelected(Group? group)
    {
        if (group == null)
            await throwBusinessException(GroupsBusinessMessages.GroupNotExists);
    }

    public async Task GroupIdShouldExistWhenSelected(int id, CancellationToken cancellationToken)
    {
        Group? group = await _groupRepository.GetAsync(
            predicate: g => g.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await GroupShouldExistWhenSelected(group);
    }
}