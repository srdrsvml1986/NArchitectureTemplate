using Application.Features.GroupClaims.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.GroupClaims;

public class GroupClaimManager : IGroupClaimService
{
    private readonly IGroupClaimRepository _groupClaimRepository;
    private readonly GroupClaimBusinessRules _groupClaimBusinessRules;

    public GroupClaimManager(IGroupClaimRepository groupClaimRepository, GroupClaimBusinessRules groupClaimBusinessRules)
    {
        _groupClaimRepository = groupClaimRepository;
        _groupClaimBusinessRules = groupClaimBusinessRules;
    }

    public async Task<GroupClaim?> GetAsync(
        Expression<Func<GroupClaim, bool>> predicate,
        Func<IQueryable<GroupClaim>, IIncludableQueryable<GroupClaim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        GroupClaim? groupClaim = await _groupClaimRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return groupClaim;
    }

    public async Task<IPaginate<GroupClaim>?> GetListAsync(
        Expression<Func<GroupClaim, bool>>? predicate = null,
        Func<IQueryable<GroupClaim>, IOrderedQueryable<GroupClaim>>? orderBy = null,
        Func<IQueryable<GroupClaim>, IIncludableQueryable<GroupClaim, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<GroupClaim> groupClaimList = await _groupClaimRepository.GetListAsync(
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

    public async Task<GroupClaim> AddAsync(GroupClaim groupClaim)
    {
        GroupClaim addedGroupClaim = await _groupClaimRepository.AddAsync(groupClaim);

        return addedGroupClaim;
    }

    public async Task<GroupClaim> UpdateAsync(GroupClaim groupClaim)
    {
        GroupClaim updatedGroupClaim = await _groupClaimRepository.UpdateAsync(groupClaim);

        return updatedGroupClaim;
    }

    public async Task<GroupClaim> DeleteAsync(GroupClaim groupClaim, bool permanent = false)
    {
        GroupClaim deletedGroupClaim = await _groupClaimRepository.DeleteAsync(groupClaim);

        return deletedGroupClaim;
    }
}
