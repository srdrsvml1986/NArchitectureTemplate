using System.Linq.Expressions;
using Application.Features.UserClaims.Rules;
using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Services.UserOperationClaims;

public class UserUserOperationClaimManager : IUserOperationClaimService
{
    private readonly IUserClaimRepository _userUserOperationClaimRepository;
    private readonly UserClaimBusinessRules _userUserOperationClaimBusinessRules;

    public UserUserOperationClaimManager(
        IUserClaimRepository userUserOperationClaimRepository,
        UserClaimBusinessRules userUserOperationClaimBusinessRules
    )
    {
        _userUserOperationClaimRepository = userUserOperationClaimRepository;
        _userUserOperationClaimBusinessRules = userUserOperationClaimBusinessRules;
    }

    public async Task<UserClaim?> GetAsync(
        Expression<Func<UserClaim, bool>> predicate,
        Func<IQueryable<UserClaim>, IIncludableQueryable<UserClaim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        UserClaim? userUserOperationClaim = await _userUserOperationClaimRepository.GetAsync(
            predicate,
            include,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return userUserOperationClaim;
    }

    public async Task<IPaginate<UserClaim>?> GetListAsync(
        Expression<Func<UserClaim, bool>>? predicate = null,
        Func<IQueryable<UserClaim>, IOrderedQueryable<UserClaim>>? orderBy = null,
        Func<IQueryable<UserClaim>, IIncludableQueryable<UserClaim, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<UserClaim> userUserOperationClaimList = await _userUserOperationClaimRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return userUserOperationClaimList;
    }

    public async Task<UserClaim> AddAsync(UserClaim userUserOperationClaim)
    {
        await _userUserOperationClaimBusinessRules.UserShouldNotHasClaimAlreadyWhenInsert(
            userUserOperationClaim.UserId,
            userUserOperationClaim.ClaimId
        );

        UserClaim addedUserOperationClaim = await _userUserOperationClaimRepository.AddAsync(userUserOperationClaim);

        return addedUserOperationClaim;
    }

    public async Task<UserClaim> UpdateAsync(UserClaim userUserOperationClaim)
    {
        await _userUserOperationClaimBusinessRules.UserShouldNotHasClaimAlreadyWhenUpdated(
            userUserOperationClaim.Id,
            userUserOperationClaim.UserId,
            userUserOperationClaim.ClaimId
        );

        UserClaim updatedUserOperationClaim = await _userUserOperationClaimRepository.UpdateAsync(
            userUserOperationClaim
        );

        return updatedUserOperationClaim;
    }

    public async Task<UserClaim> DeleteAsync(UserClaim userUserOperationClaim, bool permanent = false)
    {
        UserClaim deletedUserOperationClaim = await _userUserOperationClaimRepository.DeleteAsync(
            userUserOperationClaim
        );

        return deletedUserOperationClaim;
    }
}
