using Application.Features.RoleOperationClaims.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.RoleOperationClaims;

public class RoleOperationClaimService : IRoleOperationClaimService
{
    private readonly IRoleOperationClaimRepository _roleClaimRepository;
    private readonly RoleOperationClaimBusinessRules _roleClaimBusinessRules;

    public RoleOperationClaimService(IRoleOperationClaimRepository roleClaimRepository, RoleOperationClaimBusinessRules roleClaimBusinessRules)
    {
        _roleClaimRepository = roleClaimRepository;
        _roleClaimBusinessRules = roleClaimBusinessRules;
    }

    public async Task<RoleOperationClaim?> GetAsync(
        Expression<Func<RoleOperationClaim, bool>> predicate,
        Func<IQueryable<RoleOperationClaim>, IIncludableQueryable<RoleOperationClaim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        RoleOperationClaim? roleClaim = await _roleClaimRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return roleClaim;
    }

    public async Task<IPaginate<RoleOperationClaim>?> GetListAsync(
        Expression<Func<RoleOperationClaim, bool>>? predicate = null,
        Func<IQueryable<RoleOperationClaim>, IOrderedQueryable<RoleOperationClaim>>? orderBy = null,
        Func<IQueryable<RoleOperationClaim>, IIncludableQueryable<RoleOperationClaim, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<RoleOperationClaim> roleClaimList = await _roleClaimRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return roleClaimList;
    }

    public async Task<RoleOperationClaim> AddAsync(RoleOperationClaim roleClaim)
    {
        RoleOperationClaim addedRoleClaim = await _roleClaimRepository.AddAsync(roleClaim);

        return addedRoleClaim;
    }

    public async Task<RoleOperationClaim> UpdateAsync(RoleOperationClaim roleClaim)
    {
        RoleOperationClaim updatedRoleClaim = await _roleClaimRepository.UpdateAsync(roleClaim);

        return updatedRoleClaim;
    }

    public async Task<RoleOperationClaim> DeleteAsync(RoleOperationClaim roleClaim, bool permanent = false)
    {
        RoleOperationClaim deletedRoleClaim = await _roleClaimRepository.DeleteAsync(roleClaim);

        return deletedRoleClaim;
    }
}
