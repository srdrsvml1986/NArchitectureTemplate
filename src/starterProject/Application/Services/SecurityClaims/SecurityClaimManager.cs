using System.Linq.Expressions;
using Application.Features.SecurityClaims.Rules;
using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Services.OperationClaims;

public class SecurityClaimManager : ISecurityClaimService
{
    private readonly ISecurityClaimRepository _operationClaimRepository;
    private readonly SecurityClaimBusinessRules _operationClaimBusinessRules;

    public SecurityClaimManager(
        ISecurityClaimRepository operationClaimRepository,
        SecurityClaimBusinessRules operationClaimBusinessRules
    )
    {
        _operationClaimRepository = operationClaimRepository;
        _operationClaimBusinessRules = operationClaimBusinessRules;
    }

    public async Task<SecurityClaim?> GetAsync(
        Expression<Func<SecurityClaim, bool>> predicate,
        Func<IQueryable<SecurityClaim>, IIncludableQueryable<SecurityClaim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        SecurityClaim? operationClaim = await _operationClaimRepository.GetAsync(
            predicate,
            include,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return operationClaim;
    }

    public async Task<IPaginate<SecurityClaim>?> GetListAsync(
        Expression<Func<SecurityClaim, bool>>? predicate = null,
        Func<IQueryable<SecurityClaim>, IOrderedQueryable<SecurityClaim>>? orderBy = null,
        Func<IQueryable<SecurityClaim>, IIncludableQueryable<SecurityClaim, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<SecurityClaim> operationClaimList = await _operationClaimRepository.GetListAsync(
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

    public async Task<SecurityClaim> AddAsync(SecurityClaim operationClaim)
    {
        await _operationClaimBusinessRules.SecurityClaimNameShouldNotExistWhenCreating(operationClaim.Name);

        SecurityClaim addedOperationClaim = await _operationClaimRepository.AddAsync(operationClaim);

        return addedOperationClaim;
    }

    public async Task<SecurityClaim> UpdateAsync(SecurityClaim operationClaim)
    {
        await _operationClaimBusinessRules.SecurityClaimNameShouldNotExistWhenUpdating(operationClaim.Id, operationClaim.Name);

        SecurityClaim updatedOperationClaim = await _operationClaimRepository.UpdateAsync(operationClaim);

        return updatedOperationClaim;
    }

    public async Task<SecurityClaim> DeleteAsync(SecurityClaim operationClaim, bool permanent = false)
    {
        SecurityClaim deletedOperationClaim = await _operationClaimRepository.DeleteAsync(operationClaim);

        return deletedOperationClaim;
    }
}
