using NArchitectureTemplate.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.GroupOperationClaims;

public interface IGroupOperationClaimService
{
    Task<GroupOperationClaim?> GetAsync(
        Expression<Func<GroupOperationClaim, bool>> predicate,
        Func<IQueryable<GroupOperationClaim>, IIncludableQueryable<GroupOperationClaim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<GroupOperationClaim>?> GetListAsync(
        Expression<Func<GroupOperationClaim, bool>>? predicate = null,
        Func<IQueryable<GroupOperationClaim>, IOrderedQueryable<GroupOperationClaim>>? orderBy = null,
        Func<IQueryable<GroupOperationClaim>, IIncludableQueryable<GroupOperationClaim, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<GroupOperationClaim> AddAsync(GroupOperationClaim groupClaim);
    Task<GroupOperationClaim> UpdateAsync(GroupOperationClaim groupClaim);
    Task<GroupOperationClaim> DeleteAsync(GroupOperationClaim groupClaim, bool permanent = false);
}
