using Application.Features.RoleClaims.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.RoleClaims;

public class RoleClaimManager : IRoleClaimService
{
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly RoleClaimBusinessRules _roleClaimBusinessRules;

    public RoleClaimManager(IRoleClaimRepository roleClaimRepository, RoleClaimBusinessRules roleClaimBusinessRules)
    {
        _roleClaimRepository = roleClaimRepository;
        _roleClaimBusinessRules = roleClaimBusinessRules;
    }

    public async Task<RoleClaim?> GetAsync(
        Expression<Func<RoleClaim, bool>> predicate,
        Func<IQueryable<RoleClaim>, IIncludableQueryable<RoleClaim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        RoleClaim? roleClaim = await _roleClaimRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return roleClaim;
    }

    public async Task<IPaginate<RoleClaim>?> GetListAsync(
        Expression<Func<RoleClaim, bool>>? predicate = null,
        Func<IQueryable<RoleClaim>, IOrderedQueryable<RoleClaim>>? orderBy = null,
        Func<IQueryable<RoleClaim>, IIncludableQueryable<RoleClaim, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<RoleClaim> roleClaimList = await _roleClaimRepository.GetListAsync(
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

    public async Task<RoleClaim> AddAsync(RoleClaim roleClaim)
    {
        RoleClaim addedRoleClaim = await _roleClaimRepository.AddAsync(roleClaim);

        return addedRoleClaim;
    }

    public async Task<RoleClaim> UpdateAsync(RoleClaim roleClaim)
    {
        RoleClaim updatedRoleClaim = await _roleClaimRepository.UpdateAsync(roleClaim);

        return updatedRoleClaim;
    }

    public async Task<RoleClaim> DeleteAsync(RoleClaim roleClaim, bool permanent = false)
    {
        RoleClaim deletedRoleClaim = await _roleClaimRepository.DeleteAsync(roleClaim);

        return deletedRoleClaim;
    }
}
