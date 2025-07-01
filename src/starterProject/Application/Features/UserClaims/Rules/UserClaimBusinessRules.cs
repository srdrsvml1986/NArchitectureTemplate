using Application.Features.UserClaims.Constants;
using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;

namespace Application.Features.UserClaims.Rules;

public class UserClaimBusinessRules : BaseBusinessRules
{
    private readonly IUserClaimRepository _userClaimRepository;
    private readonly ILocalizationService _localizationService;

    public UserClaimBusinessRules(
        IUserClaimRepository userClaimRepository,
        ILocalizationService localizationService
    )
    {
        _userClaimRepository = userClaimRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, UserClaimsMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task UserClaimShouldExistWhenSelected(UserClaim? userClaim)
    {
        if (userClaim == null)
            await throwBusinessException(UserClaimsMessages.UserClaimNotExists);
    }

    public async Task UserClaimIdShouldExistWhenSelected(Guid id)
    {
        bool doesExist = await _userClaimRepository.AnyAsync(predicate: b => b.Id == id);
        if (!doesExist)
            await throwBusinessException(UserClaimsMessages.UserClaimNotExists);
    }

    public async Task UserClaimShouldNotExistWhenSelected(UserClaim? userClaim)
    {
        if (userClaim != null)
            await throwBusinessException(UserClaimsMessages.UserClaimAlreadyExists);
    }

    public async Task UserShouldNotHasClaimAlreadyWhenInsert(Guid userId, int claimId)
    {
        bool doesExist = await _userClaimRepository.AnyAsync(u =>
            u.UserId == userId && u.ClaimId == claimId
        );
        if (doesExist)
            await throwBusinessException(UserClaimsMessages.UserClaimAlreadyExists);
    }

    public async Task UserShouldNotHasClaimAlreadyWhenUpdated(Guid id, Guid userId, int claimId)
    {
        bool doesExist = await _userClaimRepository.AnyAsync(predicate: uoc =>
            uoc.Id == id && uoc.UserId == userId && uoc.ClaimId == claimId
        );
        if (doesExist)
            await throwBusinessException(UserClaimsMessages.UserClaimAlreadyExists);
    }
}
