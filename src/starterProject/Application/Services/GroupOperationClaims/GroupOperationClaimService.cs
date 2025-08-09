using Application.Features.GroupOperationClaims.Rules;
using Application.Services.Repositories;
using NArchitectureTemplate.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.GroupOperationClaims;

public class GroupOperationClaimService : IGroupOperationClaimService
{
    private readonly IGroupOperationClaimRepository _groupClaimRepository;
    private readonly GroupOperationClaimBusinessRules _groupClaimBusinessRules;

    public GroupOperationClaimService(IGroupOperationClaimRepository groupClaimRepository, GroupOperationClaimBusinessRules groupClaimBusinessRules)
    {
        _groupClaimRepository = groupClaimRepository;
        _groupClaimBusinessRules = groupClaimBusinessRules;
    }

    public async Task<GroupOperationClaim?> GetAsync(
        Expression<Func<GroupOperationClaim, bool>> predicate,
        Func<IQueryable<GroupOperationClaim>, IIncludableQueryable<GroupOperationClaim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        GroupOperationClaim? groupClaim = await _groupClaimRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return groupClaim;
    }

    public async Task<IPaginate<GroupOperationClaim>?> GetListAsync(
        Expression<Func<GroupOperationClaim, bool>>? predicate = null,
        Func<IQueryable<GroupOperationClaim>, IOrderedQueryable<GroupOperationClaim>>? orderBy = null,
        Func<IQueryable<GroupOperationClaim>, IIncludableQueryable<GroupOperationClaim, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<GroupOperationClaim> groupClaimList = await _groupClaimRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return groupClaimList;
    }

    public async Task<GroupOperationClaim> AddAsync(GroupOperationClaim groupClaim)
    {
        GroupOperationClaim addedGroupClaim = await _groupClaimRepository.AddAsync(groupClaim);

        return addedGroupClaim;
    }

    public async Task<GroupOperationClaim> UpdateAsync(GroupOperationClaim groupClaim)
    {
        GroupOperationClaim updatedGroupClaim = await _groupClaimRepository.UpdateAsync(groupClaim);

        return updatedGroupClaim;
    }

    public async Task<GroupOperationClaim> DeleteAsync(GroupOperationClaim groupClaim, bool permanent = false)
    {
        GroupOperationClaim deletedGroupClaim = await _groupClaimRepository.DeleteAsync(groupClaim);

        return deletedGroupClaim;
    }
}
