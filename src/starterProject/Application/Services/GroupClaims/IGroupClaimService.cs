using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.GroupClaims;

public interface IGroupClaimService
{
    Task<GroupClaim?> GetAsync(
        Expression<Func<GroupClaim, bool>> predicate,
        Func<IQueryable<GroupClaim>, IIncludableQueryable<GroupClaim, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<GroupClaim>?> GetListAsync(
        Expression<Func<GroupClaim, bool>>? predicate = null,
        Func<IQueryable<GroupClaim>, IOrderedQueryable<GroupClaim>>? orderBy = null,
        Func<IQueryable<GroupClaim>, IIncludableQueryable<GroupClaim, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<GroupClaim> AddAsync(GroupClaim groupClaim);
    Task<GroupClaim> UpdateAsync(GroupClaim groupClaim);
    Task<GroupClaim> DeleteAsync(GroupClaim groupClaim, bool permanent = false);
}
