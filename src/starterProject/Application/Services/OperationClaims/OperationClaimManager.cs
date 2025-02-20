using System.Linq.Expressions;
using Application.Features.Claims.Rules;
using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Services.OperationClaims;

public class OperationClaimManager : IOperationClaimService
{
    private readonly IClaimRepository _operationClaimRepository;
    private readonly ClaimBusinessRules _operationClaimBusinessRules;

    public OperationClaimManager(
        IClaimRepository operationClaimRepository,
        ClaimBusinessRules operationClaimBusinessRules
    )
    {
        _operationClaimRepository = operationClaimRepository;
        _operationClaimBusinessRules = operationClaimBusinessRules;
    }

    public async Task<Claim?> GetAsync(
        Expression<Func<Claim, bool>> predicate,
        Func<IQueryable<Claim>, IIncludableQueryable<Claim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        Claim? operationClaim = await _operationClaimRepository.GetAsync(
            predicate,
            include,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return operationClaim;
    }

    public async Task<IPaginate<Claim>?> GetListAsync(
        Expression<Func<Claim, bool>>? predicate = null,
        Func<IQueryable<Claim>, IOrderedQueryable<Claim>>? orderBy = null,
        Func<IQueryable<Claim>, IIncludableQueryable<Claim, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<Claim> operationClaimList = await _operationClaimRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return operationClaimList;
    }

    public async Task<Claim> AddAsync(Claim operationClaim)
    {
        await _operationClaimBusinessRules.OperationClaimNameShouldNotExistWhenCreating(operationClaim.Name);

        Claim addedOperationClaim = await _operationClaimRepository.AddAsync(operationClaim);

        return addedOperationClaim;
    }

    public async Task<Claim> UpdateAsync(Claim operationClaim)
    {
        await _operationClaimBusinessRules.ClaimNameShouldNotExistWhenUpdating(operationClaim.Id, operationClaim.Name);

        Claim updatedOperationClaim = await _operationClaimRepository.UpdateAsync(operationClaim);

        return updatedOperationClaim;
    }

    public async Task<Claim> DeleteAsync(Claim operationClaim, bool permanent = false)
    {
        Claim deletedOperationClaim = await _operationClaimRepository.DeleteAsync(operationClaim);

        return deletedOperationClaim;
    }
}
