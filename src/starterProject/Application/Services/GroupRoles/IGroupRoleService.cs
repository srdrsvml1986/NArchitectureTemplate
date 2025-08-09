using NArchitectureTemplate.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.GroupRoles;

public interface IGroupRoleService
{
    Task<GroupRole?> GetAsync(
        Expression<Func<GroupRole, bool>> predicate,
        Func<IQueryable<GroupRole>, IIncludableQueryable<GroupRole, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<GroupRole>?> GetListAsync(
        Expression<Func<GroupRole, bool>>? predicate = null,
        Func<IQueryable<GroupRole>, IOrderedQueryable<GroupRole>>? orderBy = null,
        Func<IQueryable<GroupRole>, IIncludableQueryable<GroupRole, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<GroupRole> AddAsync(GroupRole groupRole);
    Task<GroupRole> UpdateAsync(GroupRole groupRole);
    Task<GroupRole> DeleteAsync(GroupRole groupRole, bool permanent = false);
}
