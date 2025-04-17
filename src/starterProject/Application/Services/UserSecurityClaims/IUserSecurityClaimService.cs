using System.Linq.Expressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Services.UserSecurityClaims;

public interface IUserSecurityClaimService
{
    Task<UserSecurityClaim?> GetAsync(
        Expression<Func<UserSecurityClaim, bool>> predicate,
        Func<IQueryable<UserSecurityClaim>, IIncludableQueryable<UserSecurityClaim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<IPaginate<UserSecurityClaim>?> GetListAsync(
        Expression<Func<UserSecurityClaim, bool>>? predicate = null,
        Func<IQueryable<UserSecurityClaim>, IOrderedQueryable<UserSecurityClaim>>? orderBy = null,
        Func<IQueryable<UserSecurityClaim>, IIncludableQueryable<UserSecurityClaim, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<UserSecurityClaim> AddAsync(UserSecurityClaim userOperationClaim);
    Task<UserSecurityClaim> UpdateAsync(UserSecurityClaim userOperationClaim);
    Task<UserSecurityClaim> DeleteAsync(UserSecurityClaim userOperationClaim, bool permanent = false);
}
