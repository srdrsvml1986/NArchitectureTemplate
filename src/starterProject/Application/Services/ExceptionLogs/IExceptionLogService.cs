using NArchitectureTemplate.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.ExceptionLogs;

public interface IExceptionLogService
{
    Task<ExceptionLog?> GetAsync(
        Expression<Func<ExceptionLog, bool>> predicate,
        Func<IQueryable<ExceptionLog>, IIncludableQueryable<ExceptionLog, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<ExceptionLog>?> GetListAsync(
        Expression<Func<ExceptionLog, bool>>? predicate = null,
        Func<IQueryable<ExceptionLog>, IOrderedQueryable<ExceptionLog>>? orderBy = null,
        Func<IQueryable<ExceptionLog>, IIncludableQueryable<ExceptionLog, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<ExceptionLog> AddAsync(ExceptionLog exceptionLog, bool enableTracking = true);
    Task<ICollection<ExceptionLog>> AddRangeAsync(ICollection<ExceptionLog> exceptionLog, bool enableTracking = true);
    Task<ExceptionLog> UpdateAsync(ExceptionLog exceptionLog, bool enableTracking = true);
    Task<ICollection<ExceptionLog>> UpdateRangeAsync(ICollection<ExceptionLog> exceptionLog, bool enableTracking = true);
    Task<ExceptionLog> DeleteAsync(ExceptionLog exceptionLog, bool permanent = false);
}
