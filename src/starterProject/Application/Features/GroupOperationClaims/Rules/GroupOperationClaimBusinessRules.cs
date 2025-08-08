using Application.Features.GroupOperationClaims.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.GroupOperationClaims.Rules;

public class GroupOperationClaimBusinessRules : BaseBusinessRules
{
    private readonly IGroupOperationClaimRepository _groupClaimRepository;
    private readonly ILocalizationService _localizationService;

    public GroupOperationClaimBusinessRules(IGroupOperationClaimRepository groupClaimRepository, ILocalizationService localizationService)
    {
        _groupClaimRepository = groupClaimRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, GroupOperationClaimsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task GroupClaimShouldExistWhenSelected(GroupOperationClaim? groupClaim)
    {
        if (groupClaim == null)
            await throwBusinessException(GroupOperationClaimsBusinessMessages.GroupClaimNotExists);
    }

    public async Task GroupClaimIdShouldExistWhenSelected(int id, CancellationToken cancellationToken)
    {
        GroupOperationClaim? groupClaim = await _groupClaimRepository.GetAsync(
            predicate: gc => gc.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await GroupClaimShouldExistWhenSelected(groupClaim);
    }
}