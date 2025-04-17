using System.Linq.Expressions;
using Application.Features.UserSecurityClaims.Rules;
using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Services.UserSecurityClaims;

public class UserUserSecurityClaimManager : IUserSecurityClaimService
{
    private readonly IUserSecurityClaimRepository _userUserSecurityClaimRepository;
    private readonly UserSecurityClaimBusinessRules _userUserSecurityClaimBusinessRules;

    public UserUserSecurityClaimManager(
        IUserSecurityClaimRepository userUserSecurityClaimRepository,
        UserSecurityClaimBusinessRules userUserSecurityClaimBusinessRules
    )
    {
        _userUserSecurityClaimRepository = userUserSecurityClaimRepository;
        _userUserSecurityClaimBusinessRules = userUserSecurityClaimBusinessRules;
    }

    public async Task<UserSecurityClaim?> GetAsync(
        Expression<Func<UserSecurityClaim, bool>> predicate,
        Func<IQueryable<UserSecurityClaim>, IIncludableQueryable<UserSecurityClaim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        UserSecurityClaim? userUserSecurityClaim = await _userUserSecurityClaimRepository.GetAsync(
            predicate,
            include,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return userUserSecurityClaim;
    }

    public async Task<IPaginate<UserSecurityClaim>?> GetListAsync(
        Expression<Func<UserSecurityClaim, bool>>? predicate = null,
        Func<IQueryable<UserSecurityClaim>, IOrderedQueryable<UserSecurityClaim>>? orderBy = null,
        Func<IQueryable<UserSecurityClaim>, IIncludableQueryable<UserSecurityClaim, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<UserSecurityClaim> userUserSecurityClaimList = await _userUserSecurityClaimRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return userUserSecurityClaimList;
    }

    public async Task<UserSecurityClaim> AddAsync(UserSecurityClaim userUserSecurityClaim)
    {
        await _userUserSecurityClaimBusinessRules.UserShouldNotHasClaimAlreadyWhenInsert(
            userUserSecurityClaim.UserId,
            userUserSecurityClaim.ClaimId
        );

        UserSecurityClaim addedUserSecurityClaim = await _userUserSecurityClaimRepository.AddAsync(userUserSecurityClaim);

        return addedUserSecurityClaim;
    }

    public async Task<UserSecurityClaim> UpdateAsync(UserSecurityClaim userUserSecurityClaim)
    {
        await _userUserSecurityClaimBusinessRules.UserShouldNotHasClaimAlreadyWhenUpdated(
            userUserSecurityClaim.Id,
            userUserSecurityClaim.UserId,
            userUserSecurityClaim.ClaimId
        );

        UserSecurityClaim updatedUserSecurityClaim = await _userUserSecurityClaimRepository.UpdateAsync(
            userUserSecurityClaim
        );

        return updatedUserSecurityClaim;
    }

    public async Task<UserSecurityClaim> DeleteAsync(UserSecurityClaim userUserSecurityClaim, bool permanent = false)
    {
        UserSecurityClaim deletedUserSecurityClaim = await _userUserSecurityClaimRepository.DeleteAsync(
            userUserSecurityClaim
        );

        return deletedUserSecurityClaim;
    }
}
