using Application.Features.GroupRoles.Constants;
using Application.Services.Repositories;
using NArchitectureTemplate.Core.Application.Rules;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.Types;
using NArchitectureTemplate.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.GroupRoles.Rules;

public class GroupRoleBusinessRules : BaseBusinessRules
{
    private readonly IGroupRoleRepository _groupRoleRepository;
    private readonly ILocalizationService _localizationService;

    public GroupRoleBusinessRules(IGroupRoleRepository groupRoleRepository, ILocalizationService localizationService)
    {
        _groupRoleRepository = groupRoleRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, GroupRolesBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task GroupRoleShouldExistWhenSelected(GroupRole? groupRole)
    {
        if (groupRole == null)
            await throwBusinessException(GroupRolesBusinessMessages.GroupRoleNotExists);
    }

    public async Task GroupRoleIdShouldExistWhenSelected(int id, CancellationToken cancellationToken)
    {
        GroupRole? groupRole = await _groupRoleRepository.GetAsync(
            predicate: gr => gr.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await GroupRoleShouldExistWhenSelected(groupRole);
    }
}