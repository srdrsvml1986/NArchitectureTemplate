using Application.Features.SecurityClaims.Constants;
using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;

namespace Application.Features.SecurityClaims.Rules;

public class SecurityClaimBusinessRules : BaseBusinessRules
{
    private readonly ISecurityClaimRepository _operationClaimRepository;
    private readonly ILocalizationService _localizationService;

    public SecurityClaimBusinessRules(
        ISecurityClaimRepository operationClaimRepository,
        ILocalizationService localizationService
    )
    {
        _operationClaimRepository = operationClaimRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, SecurityClaimsMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task SecurityClaimShouldExistWhenSelected(SecurityClaim? operationClaim)
    {
        if (operationClaim == null)
            await throwBusinessException(SecurityClaimsMessages.NotExists);
    }

    public async Task OperationClaimIdShouldExistWhenSelected(int id)
    {
        bool doesExist = await _operationClaimRepository.AnyAsync(predicate: b => b.Id == id);
        if (doesExist)
            await throwBusinessException(SecurityClaimsMessages.NotExists);
    }

    public async Task SecurityClaimNameShouldNotExistWhenCreating(string name)
    {
        bool doesExist = await _operationClaimRepository.AnyAsync(predicate: b => b.Name == name);
        if (doesExist)
            await throwBusinessException(SecurityClaimsMessages.AlreadyExists);
    }

    public async Task SecurityClaimNameShouldNotExistWhenUpdating(int id, string name)
    {
        bool doesExist = await _operationClaimRepository.AnyAsync(predicate: b => b.Id != id && b.Name == name);
        if (doesExist)
            await throwBusinessException(SecurityClaimsMessages.AlreadyExists);
    }
}
