using Application.Features.RoleClaims.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.RoleClaims.Rules;

public class RoleClaimBusinessRules : BaseBusinessRules
{
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly ILocalizationService _localizationService;

    public RoleClaimBusinessRules(IRoleClaimRepository roleClaimRepository, ILocalizationService localizationService)
    {
        _roleClaimRepository = roleClaimRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, RoleClaimsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task RoleClaimShouldExistWhenSelected(RoleClaim? roleClaim)
    {
        if (roleClaim == null)
            await throwBusinessException(RoleClaimsBusinessMessages.RoleClaimNotExists);
    }

    public async Task RoleClaimIdShouldExistWhenSelected(int id, CancellationToken cancellationToken)
    {
        RoleClaim? roleClaim = await _roleClaimRepository.GetAsync(
            predicate: rc => rc.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await RoleClaimShouldExistWhenSelected(roleClaim);
    }
}