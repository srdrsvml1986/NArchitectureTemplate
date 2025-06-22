using System.Linq.Expressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Services.UserClaims;

public interface IUserClaimService
{
    Task<UserClaim?> GetAsync(
        Expression<Func<UserClaim, bool>> predicate,
        Func<IQueryable<UserClaim>, IIncludableQueryable<UserClaim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<IPaginate<UserClaim>?> GetListAsync(
        Expression<Func<UserClaim, bool>>? predicate = null,
        Func<IQueryable<UserClaim>, IOrderedQueryable<UserClaim>>? orderBy = null,
        Func<IQueryable<UserClaim>, IIncludableQueryable<UserClaim, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<UserClaim> AddAsync(UserClaim userOperationClaim);
    Task<UserClaim> UpdateAsync(UserClaim userOperationClaim);
    Task<UserClaim> DeleteAsync(UserClaim userOperationClaim, bool permanent = false);
}
