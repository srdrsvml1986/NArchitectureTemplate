using System.Linq.Expressions;
using Application.Features.UserOperationClaims.Rules;
using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using NArchitectureTemplate.Core.Persistence.Paging;

namespace Application.Services.UserOperationClaims;

public class UserOperationClaimService : IUserOperationClaimService
{
    private readonly IUserOperationClaimRepository _userUserClaimRepository;
    private readonly UserOperationClaimBusinessRules _userUserClaimBusinessRules;

    public UserOperationClaimService(
        IUserOperationClaimRepository userUserClaimRepository,
        UserOperationClaimBusinessRules userUserClaimBusinessRules
    )
    {
        _userUserClaimRepository = userUserClaimRepository;
        _userUserClaimBusinessRules = userUserClaimBusinessRules;
    }

    public async Task<UserOperationClaim?> GetAsync(
        Expression<Func<UserOperationClaim, bool>> predicate,
        Func<IQueryable<UserOperationClaim>, IIncludableQueryable<UserOperationClaim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        UserOperationClaim? userUserClaim = await _userUserClaimRepository.GetAsync(
            predicate,
            include,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return userUserClaim;
    }

    public async Task<IPaginate<UserOperationClaim>?> GetListAsync(
        Expression<Func<UserOperationClaim, bool>>? predicate = null,
        Func<IQueryable<UserOperationClaim>, IOrderedQueryable<UserOperationClaim>>? orderBy = null,
        Func<IQueryable<UserOperationClaim>, IIncludableQueryable<UserOperationClaim, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<UserOperationClaim> userUserClaimList = await _userUserClaimRepository.GetListAsync(
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

    public async Task<UserOperationClaim> AddAsync(UserOperationClaim userUserClaim)
    {
        await _userUserClaimBusinessRules.UserShouldNotHasClaimAlreadyWhenInsert(
            userUserClaim.UserId,
            userUserClaim.OperationClaimId
        );

        UserOperationClaim addedUserClaim = await _userUserClaimRepository.AddAsync(userUserClaim);

        return addedUserClaim;
    }

    public async Task<UserOperationClaim> UpdateAsync(UserOperationClaim userUserClaim)
    {
        await _userUserClaimBusinessRules.UserShouldNotHasClaimAlreadyWhenUpdated(
            userUserClaim.Id,
            userUserClaim.UserId,
            userUserClaim.OperationClaimId
        );

        UserOperationClaim updatedUserClaim = await _userUserClaimRepository.UpdateAsync(
            userUserClaim
        );

        return updatedUserClaim;
    }

    public async Task<UserOperationClaim> DeleteAsync(UserOperationClaim userUserClaim, bool permanent = false)
    {
        UserOperationClaim deletedUserClaim = await _userUserClaimRepository.DeleteAsync(
            userUserClaim
        );

        return deletedUserClaim;
    }
}
