﻿using System.Linq.Expressions;
using Application.Features.OperationClaims.Rules;
using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Services.OperationClaims;

public class OperationClaimService : IOperationClaimService
{
    private readonly IOperationClaimRepository _operationClaimRepository;
    private readonly OperationClaimBusinessRules _operationClaimBusinessRules;

    public OperationClaimService(
        IOperationClaimRepository operationClaimRepository,
        OperationClaimBusinessRules operationClaimBusinessRules
    )
    {
        _operationClaimRepository = operationClaimRepository;
        _operationClaimBusinessRules = operationClaimBusinessRules;
    }

    public async Task<OperationClaim?> GetAsync(
        Expression<Func<OperationClaim, bool>> predicate,
        Func<IQueryable<OperationClaim>, IIncludableQueryable<OperationClaim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        OperationClaim? operationClaim = await _operationClaimRepository.GetAsync(
            predicate,
            include,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return operationClaim;
    }

    public async Task<IPaginate<OperationClaim>?> GetListAsync(
        Expression<Func<OperationClaim, bool>>? predicate = null,
        Func<IQueryable<OperationClaim>, IOrderedQueryable<OperationClaim>>? orderBy = null,
        Func<IQueryable<OperationClaim>, IIncludableQueryable<OperationClaim, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<OperationClaim> operationClaimList = await _operationClaimRepository.GetListAsync(
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

    public async Task<OperationClaim> AddAsync(OperationClaim operationClaim)
    {
        await _operationClaimBusinessRules.ClaimNameShouldNotExistWhenCreating(operationClaim.Name);

        OperationClaim addedOperationClaim = await _operationClaimRepository.AddAsync(operationClaim);

        return addedOperationClaim;
    }

    public async Task<OperationClaim> UpdateAsync(OperationClaim operationClaim)
    {
        await _operationClaimBusinessRules.ClaimNameShouldNotExistWhenUpdating(operationClaim.Id, operationClaim.Name);

        OperationClaim updatedOperationClaim = await _operationClaimRepository.UpdateAsync(operationClaim);

        return updatedOperationClaim;
    }

    public async Task<OperationClaim> DeleteAsync(OperationClaim operationClaim, bool permanent = false)
    {
        OperationClaim deletedOperationClaim = await _operationClaimRepository.DeleteAsync(operationClaim);

        return deletedOperationClaim;
    }
}
