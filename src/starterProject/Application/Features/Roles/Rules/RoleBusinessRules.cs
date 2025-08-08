using Application.Features.Roles.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.Roles.Rules;

public class RoleBusinessRules : BaseBusinessRules
{
    private readonly IRoleRepository _roleRepository;
    private readonly ILocalizationService _localizationService;

    public RoleBusinessRules(IRoleRepository roleRepository, ILocalizationService localizationService)
    {
        _roleRepository = roleRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, RolesBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task RoleShouldExistWhenSelected(Role? role)
    {
        if (role == null)
            await throwBusinessException(RolesBusinessMessages.RoleNotExists);
    }

    public async Task RoleIdShouldExistWhenSelected(int id, CancellationToken cancellationToken)
    {
        Role? role = await _roleRepository.GetAsync(
            predicate: r => r.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await RoleShouldExistWhenSelected(role);
    }
}