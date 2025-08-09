using Application.Features.Groups.Constants;
using Application.Services.Repositories;
using NArchitectureTemplate.Core.Application.Rules;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.Types;
using NArchitectureTemplate.Core.Localization.Abstraction;
using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Paging;

namespace Application.Features.Groups.Rules;

public class GroupBusinessRules : BaseBusinessRules
{
    private readonly IGroupRepository _groupRepository;
    private readonly ILocalizationService _localizationService;
    private readonly IOperationClaimRepository _operationClaimRepository;
    public GroupBusinessRules(IGroupRepository groupRepository, ILocalizationService localizationService, IOperationClaimRepository operationClaimRepository)
    {
        _groupRepository = groupRepository;
        _localizationService = localizationService;
        _operationClaimRepository = operationClaimRepository;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, GroupsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task GroupShouldExistWhenSelected(Group? group)
    {
        if (group == null)
            await throwBusinessException(GroupsBusinessMessages.GroupNotExists);
    }

    public async Task GroupIdShouldExistWhenSelected(int id, CancellationToken cancellationToken)
    {
        Group? group = await _groupRepository.GetAsync(
            predicate: g => g.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await GroupShouldExistWhenSelected(group);
    }
    public async Task ClaimsShouldExistWhenSelected(IList<int> claimIds, CancellationToken cancellationToken)
    {
        // Get all claims that match the provided IDs
        IPaginate<OperationClaim> claims = await _operationClaimRepository.GetListAsync(
            predicate: x => claimIds.Contains(x.Id),
            cancellationToken: cancellationToken
        );

        // Check if all claim IDs were found
        if (claims.Items.Count != claimIds.Count)
        {
            // Find missing claim IDs
            var foundClaimIds = claims.Items.Select(c => c.Id).ToList();
            var missingIds = claimIds.Except(foundClaimIds).ToList();

            throw new NotFoundException(
                $"The following claim IDs were not found: {string.Join(", ", missingIds)}"
            );
        }
    }
}