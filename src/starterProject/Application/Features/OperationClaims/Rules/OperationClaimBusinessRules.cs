using Application.Features.OperationClaims.Constants;
using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;

namespace Application.Features.OperationClaims.Rules;

public class OperationClaimBusinessRules : BaseBusinessRules
{
    private readonly IOperationClaimRepository _operationClaimRepository;
    private readonly ILocalizationService _localizationService;

    public OperationClaimBusinessRules(
        IOperationClaimRepository operationClaimRepository,
        ILocalizationService localizationService
    )
    {
        _operationClaimRepository = operationClaimRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, ClaimsMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task ClaimShouldExistWhenSelected(OperationClaim? operationClaim)
    {
        if (operationClaim == null)
            await throwBusinessException(ClaimsMessages.NotExists);
    }

    public async Task OperationClaimIdShouldExistWhenSelected(int id)
    {
        bool doesExist = await _operationClaimRepository.AnyAsync(predicate: b => b.Id == id);
        if (doesExist)
            await throwBusinessException(ClaimsMessages.NotExists);
    }

    public async Task ClaimNameShouldNotExistWhenCreating(string name)
    {
        bool doesExist = await _operationClaimRepository.AnyAsync(predicate: b => b.Name == name);
        if (doesExist)
            await throwBusinessException(ClaimsMessages.AlreadyExists);
    }

    public async Task ClaimNameShouldNotExistWhenUpdating(int id, string name)
    {
        bool doesExist = await _operationClaimRepository.AnyAsync(predicate: b => b.Id != id && b.Name == name);
        if (doesExist)
            await throwBusinessException(ClaimsMessages.AlreadyExists);
    }
}
