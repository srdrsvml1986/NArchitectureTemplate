using System.Linq.Expressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Services.OperationClaims;

public interface IOperationClaimService
{
    Task<SecurityClaim?> GetAsync(
        Expression<Func<SecurityClaim, bool>> predicate,
        Func<IQueryable<SecurityClaim>, IIncludableQueryable<SecurityClaim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<IPaginate<SecurityClaim>?> GetListAsync(
        Expression<Func<SecurityClaim, bool>>? predicate = null,
        Func<IQueryable<SecurityClaim>, IOrderedQueryable<SecurityClaim>>? orderBy = null,
        Func<IQueryable<SecurityClaim>, IIncludableQueryable<SecurityClaim, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<SecurityClaim> AddAsync(SecurityClaim operationClaim);
    Task<SecurityClaim> UpdateAsync(SecurityClaim operationClaim);
    Task<SecurityClaim> DeleteAsync(SecurityClaim operationClaim, bool permanent = false);
}
