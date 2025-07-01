using System.Linq.Expressions;
using Application.Features.UserClaims.Rules;
using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Services.UserClaims;

public class UserUserClaimManager : IUserClaimService
{
    private readonly IUserClaimRepository _userUserClaimRepository;
    private readonly UserClaimBusinessRules _userUserClaimBusinessRules;

    public UserUserClaimManager(
        IUserClaimRepository userUserClaimRepository,
        UserClaimBusinessRules userUserClaimBusinessRules
    )
    {
        _userUserClaimRepository = userUserClaimRepository;
        _userUserClaimBusinessRules = userUserClaimBusinessRules;
    }

    public async Task<UserClaim?> GetAsync(
        Expression<Func<UserClaim, bool>> predicate,
        Func<IQueryable<UserClaim>, IIncludableQueryable<UserClaim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        UserClaim? userUserClaim = await _userUserClaimRepository.GetAsync(
            predicate,
            include,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return userUserClaim;
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
        IPaginate<UserClaim> userUserClaimList = await _userUserClaimRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return userUserClaimList;
    }

    public async Task<UserClaim> AddAsync(UserClaim userUserClaim)
    {
        await _userUserClaimBusinessRules.UserShouldNotHasClaimAlreadyWhenInsert(
            userUserClaim.UserId,
            userUserClaim.ClaimId
        );

        UserClaim addedUserClaim = await _userUserClaimRepository.AddAsync(userUserClaim);

        return addedUserClaim;
    }

    public async Task<UserClaim> UpdateAsync(UserClaim userUserClaim)
    {
        await _userUserClaimBusinessRules.UserShouldNotHasClaimAlreadyWhenUpdated(
            userUserClaim.Id,
            userUserClaim.UserId,
            userUserClaim.ClaimId
        );

        UserClaim updatedUserClaim = await _userUserClaimRepository.UpdateAsync(
            userUserClaim
        );

        return updatedUserClaim;
    }

    public async Task<UserClaim> DeleteAsync(UserClaim userUserClaim, bool permanent = false)
    {
        UserClaim deletedUserClaim = await _userUserClaimRepository.DeleteAsync(
            userUserClaim
        );

        return deletedUserClaim;
    }
}
