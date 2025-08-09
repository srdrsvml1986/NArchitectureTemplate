using Application.Features.UserOperationClaims.Constants;
using Application.Services.Repositories;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Rules;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.Types;
using NArchitectureTemplate.Core.Localization.Abstraction;

namespace Application.Features.UserOperationClaims.Rules;

public class UserOperationClaimBusinessRules : BaseBusinessRules
{
    private readonly IUserOperationClaimRepository _userClaimRepository;
    private readonly ILocalizationService _localizationService;

    public UserOperationClaimBusinessRules(
        IUserOperationClaimRepository userClaimRepository,
        ILocalizationService localizationService
    )
    {
        _userClaimRepository = userClaimRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, UserOperationClaimMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task UserClaimShouldExistWhenSelected(UserOperationClaim? userClaim)
    {
        if (userClaim == null)
            await throwBusinessException(UserOperationClaimMessages.UserClaimNotExists);
    }

    public async Task UserClaimIdShouldExistWhenSelected(Guid id)
    {
        bool doesExist = await _userClaimRepository.AnyAsync(predicate: b => b.Id == id);
        if (!doesExist)
            await throwBusinessException(UserOperationClaimMessages.UserClaimNotExists);
    }

    public async Task UserClaimShouldNotExistWhenSelected(UserOperationClaim? userClaim)
    {
        if (userClaim != null)
            await throwBusinessException(UserOperationClaimMessages.UserClaimAlreadyExists);
    }

    public async Task UserShouldNotHasClaimAlreadyWhenInsert(Guid userId, int claimId)
    {
        bool doesExist = await _userClaimRepository.AnyAsync(u =>
            u.UserId == userId && u.OperationClaimId == claimId
        );
        if (doesExist)
            await throwBusinessException(UserOperationClaimMessages.UserClaimAlreadyExists);
    }

    public async Task UserShouldNotHasClaimAlreadyWhenUpdated(Guid id, Guid userId, int claimId)
    {
        bool doesExist = await _userClaimRepository.AnyAsync(predicate: uoc =>
            uoc.Id == id && uoc.UserId == userId && uoc.OperationClaimId == claimId
        );
        if (doesExist)
            await throwBusinessException(UserOperationClaimMessages.UserClaimAlreadyExists);
    }
}
