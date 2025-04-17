using Application.Features.UserSecurityClaims.Constants;
using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;

namespace Application.Features.UserSecurityClaims.Rules;

public class UserSecurityClaimBusinessRules : BaseBusinessRules
{
    private readonly IUserSecurityClaimRepository _userClaimRepository;
    private readonly ILocalizationService _localizationService;

    public UserSecurityClaimBusinessRules(
        IUserSecurityClaimRepository userClaimRepository,
        ILocalizationService localizationService
    )
    {
        _userClaimRepository = userClaimRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, UserSecurityClaimsMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task UserSecurityClaimShouldExistWhenSelected(UserSecurityClaim? userClaim)
    {
        if (userClaim == null)
            await throwBusinessException(UserSecurityClaimsMessages.UserSecurityClaimNotExists);
    }

    public async Task UserSecurityClaimIdShouldExistWhenSelected(Guid id)
    {
        bool doesExist = await _userClaimRepository.AnyAsync(predicate: b => b.Id == id);
        if (!doesExist)
            await throwBusinessException(UserSecurityClaimsMessages.UserSecurityClaimNotExists);
    }

    public async Task UserSecurityClaimShouldNotExistWhenSelected(UserSecurityClaim? userClaim)
    {
        if (userClaim != null)
            await throwBusinessException(UserSecurityClaimsMessages.UserSecurityClaimAlreadyExists);
    }

    public async Task UserShouldNotHasClaimAlreadyWhenInsert(Guid userId, int claimId)
    {
        bool doesExist = await _userClaimRepository.AnyAsync(u =>
            u.UserId == userId && u.SecurityClaimId == claimId
        );
        if (doesExist)
            await throwBusinessException(UserSecurityClaimsMessages.UserSecurityClaimAlreadyExists);
    }

    public async Task UserShouldNotHasClaimAlreadyWhenUpdated(Guid id, Guid userId, int claimId)
    {
        bool doesExist = await _userClaimRepository.AnyAsync(predicate: uoc =>
            uoc.Id == id && uoc.UserId == userId && uoc.SecurityClaimId == claimId
        );
        if (doesExist)
            await throwBusinessException(UserSecurityClaimsMessages.UserSecurityClaimAlreadyExists);
    }
}
