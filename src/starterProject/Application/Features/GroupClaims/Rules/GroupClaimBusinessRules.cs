using Application.Features.GroupClaims.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.GroupClaims.Rules;

public class GroupClaimBusinessRules : BaseBusinessRules
{
    private readonly IGroupClaimRepository _groupClaimRepository;
    private readonly ILocalizationService _localizationService;

    public GroupClaimBusinessRules(IGroupClaimRepository groupClaimRepository, ILocalizationService localizationService)
    {
        _groupClaimRepository = groupClaimRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, GroupClaimsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task GroupClaimShouldExistWhenSelected(GroupClaim? groupClaim)
    {
        if (groupClaim == null)
            await throwBusinessException(GroupClaimsBusinessMessages.GroupClaimNotExists);
    }

    public async Task GroupClaimIdShouldExistWhenSelected(int id, CancellationToken cancellationToken)
    {
        GroupClaim? groupClaim = await _groupClaimRepository.GetAsync(
            predicate: gc => gc.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await GroupClaimShouldExistWhenSelected(groupClaim);
    }
}