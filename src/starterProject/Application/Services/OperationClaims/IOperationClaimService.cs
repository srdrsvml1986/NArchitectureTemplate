using System.Linq.Expressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Services.OperationClaims;

public interface IOperationClaimService
{
    Task<Claim?> GetAsync(
        Expression<Func<Claim, bool>> predicate,
        Func<IQueryable<Claim>, IIncludableQueryable<Claim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<IPaginate<Claim>?> GetListAsync(
        Expression<Func<Claim, bool>>? predicate = null,
        Func<IQueryable<Claim>, IOrderedQueryable<Claim>>? orderBy = null,
        Func<IQueryable<Claim>, IIncludableQueryable<Claim, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<Claim> AddAsync(Claim operationClaim);
    Task<Claim> UpdateAsync(Claim operationClaim);
    Task<Claim> DeleteAsync(Claim operationClaim, bool permanent = false);
}
