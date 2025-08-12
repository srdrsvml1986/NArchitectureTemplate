using NArchitectureTemplate.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.Logs;

public interface ILogService
{
    Task<Log?> GetAsync(
        Expression<Func<Log, bool>> predicate,
        Func<IQueryable<Log>, IIncludableQueryable<Log, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<Log>?> GetListAsync(
        Expression<Func<Log, bool>>? predicate = null,
        Func<IQueryable<Log>, IOrderedQueryable<Log>>? orderBy = null,
        Func<IQueryable<Log>, IIncludableQueryable<Log, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<Log> AddAsync(Log log, bool enableTracking = true);
    Task<ICollection<Log>> AddRangeAsync(ICollection<Log> log, bool enableTracking = true);
    Task<Log> UpdateAsync(Log log, bool enableTracking = true);
    Task<ICollection<Log>> UpdateRangeAsync(ICollection<Log> log, bool enableTracking = true);
    Task<Log> DeleteAsync(Log log, bool permanent = false);
}
