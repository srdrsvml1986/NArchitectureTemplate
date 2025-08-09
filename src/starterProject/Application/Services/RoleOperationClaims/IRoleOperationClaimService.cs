using NArchitectureTemplate.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.RoleOperationClaims;

public interface IRoleOperationClaimService
{
    Task<RoleOperationClaim?> GetAsync(
        Expression<Func<RoleOperationClaim, bool>> predicate,
        Func<IQueryable<RoleOperationClaim>, IIncludableQueryable<RoleOperationClaim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<RoleOperationClaim>?> GetListAsync(
        Expression<Func<RoleOperationClaim, bool>>? predicate = null,
        Func<IQueryable<RoleOperationClaim>, IOrderedQueryable<RoleOperationClaim>>? orderBy = null,
        Func<IQueryable<RoleOperationClaim>, IIncludableQueryable<RoleOperationClaim, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<RoleOperationClaim> AddAsync(RoleOperationClaim roleClaim);
    Task<RoleOperationClaim> UpdateAsync(RoleOperationClaim roleClaim);
    Task<RoleOperationClaim> DeleteAsync(RoleOperationClaim roleClaim, bool permanent = false);
}
