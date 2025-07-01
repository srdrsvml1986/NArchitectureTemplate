using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.RoleClaims;

public interface IRoleClaimService
{
    Task<RoleClaim?> GetAsync(
        Expression<Func<RoleClaim, bool>> predicate,
        Func<IQueryable<RoleClaim>, IIncludableQueryable<RoleClaim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<RoleClaim>?> GetListAsync(
        Expression<Func<RoleClaim, bool>>? predicate = null,
        Func<IQueryable<RoleClaim>, IOrderedQueryable<RoleClaim>>? orderBy = null,
        Func<IQueryable<RoleClaim>, IIncludableQueryable<RoleClaim, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<RoleClaim> AddAsync(RoleClaim roleClaim);
    Task<RoleClaim> UpdateAsync(RoleClaim roleClaim);
    Task<RoleClaim> DeleteAsync(RoleClaim roleClaim, bool permanent = false);
}
