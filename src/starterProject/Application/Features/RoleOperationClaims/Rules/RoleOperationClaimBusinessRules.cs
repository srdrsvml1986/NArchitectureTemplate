using Application.Features.RoleOperationClaims.Constants;
using Application.Services.Repositories;
using NArchitectureTemplate.Core.Application.Rules;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.Types;
using NArchitectureTemplate.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.RoleOperationClaims.Rules;

public class RoleOperationClaimBusinessRules : BaseBusinessRules
{
    private readonly IRoleOperationClaimRepository _roleClaimRepository;
    private readonly ILocalizationService _localizationService;

    public RoleOperationClaimBusinessRules(IRoleOperationClaimRepository roleClaimRepository, ILocalizationService localizationService)
    {
        _roleClaimRepository = roleClaimRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, RoleOperationClaimsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task RoleClaimShouldExistWhenSelected(RoleOperationClaim? roleClaim)
    {
        if (roleClaim == null)
            await throwBusinessException(RoleOperationClaimsBusinessMessages.RoleClaimNotExists);
    }

    public async Task RoleClaimIdShouldExistWhenSelected(int id, CancellationToken cancellationToken)
    {
        RoleOperationClaim? roleClaim = await _roleClaimRepository.GetAsync(
            predicate: rc => rc.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await RoleClaimShouldExistWhenSelected(roleClaim);
    }
}