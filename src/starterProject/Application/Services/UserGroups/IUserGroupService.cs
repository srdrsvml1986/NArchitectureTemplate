using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.UserGroups;

public interface IUserGroupService
{
    Task<UserGroup?> GetAsync(
        Expression<Func<UserGroup, bool>> predicate,
        Func<IQueryable<UserGroup>, IIncludableQueryable<UserGroup, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<UserGroup>?> GetListAsync(
        Expression<Func<UserGroup, bool>>? predicate = null,
        Func<IQueryable<UserGroup>, IOrderedQueryable<UserGroup>>? orderBy = null,
        Func<IQueryable<UserGroup>, IIncludableQueryable<UserGroup, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<UserGroup> AddAsync(UserGroup userGroup);
    Task<UserGroup> UpdateAsync(UserGroup userGroup);
    Task<UserGroup> DeleteAsync(UserGroup userGroup, bool permanent = false);
}
