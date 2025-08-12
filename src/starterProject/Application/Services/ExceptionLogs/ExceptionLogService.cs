using Application.Features.ExceptionLogs.Rules;
using Application.Services.Repositories;
using NArchitectureTemplate.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.ExceptionLogs;

public class ExceptionLogService : IExceptionLogService
{
    private readonly IExceptionLogRepository _exceptionLogRepository;
    private readonly ExceptionLogBusinessRules _exceptionLogBusinessRules;

    public ExceptionLogService(IExceptionLogRepository exceptionLogRepository, ExceptionLogBusinessRules exceptionLogBusinessRules)
    {
        _exceptionLogRepository = exceptionLogRepository;
        _exceptionLogBusinessRules = exceptionLogBusinessRules;
    }

    public async Task<ExceptionLog?> GetAsync(
        Expression<Func<ExceptionLog, bool>> predicate,
        Func<IQueryable<ExceptionLog>, IIncludableQueryable<ExceptionLog, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        ExceptionLog? exceptionLog = await _exceptionLogRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return exceptionLog;
    }

    public async Task<IPaginate<ExceptionLog>?> GetListAsync(
        Expression<Func<ExceptionLog, bool>>? predicate = null,
        Func<IQueryable<ExceptionLog>, IOrderedQueryable<ExceptionLog>>? orderBy = null,
        Func<IQueryable<ExceptionLog>, IIncludableQueryable<ExceptionLog, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<ExceptionLog> exceptionLogList = await _exceptionLogRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return exceptionLogList;
    }

    public async Task<ExceptionLog> AddAsync(ExceptionLog exceptionLog, bool enableTracking = true)
    {
        ExceptionLog addedExceptionLog = await _exceptionLogRepository.AddAsync(exceptionLog);

        return addedExceptionLog;
    }

    public async Task<ICollection<ExceptionLog>> AddRangeAsync(ICollection<ExceptionLog> exceptionLog, bool enableTracking = true)
    {
        ICollection<ExceptionLog> addedExceptionLog = await _exceptionLogRepository.AddRangeAsync(exceptionLog);

        return addedExceptionLog;
    }

    public async Task<ExceptionLog> UpdateAsync(ExceptionLog exceptionLog, bool enableTracking = true)
    {
        ExceptionLog updatedExceptionLog = await _exceptionLogRepository.UpdateAsync(exceptionLog);

        return updatedExceptionLog;
    }

    public async Task<ICollection<ExceptionLog>> UpdateRangeAsync(ICollection<ExceptionLog> exceptionLog, bool enableTracking = true)
    {
        ICollection<ExceptionLog> updatedExceptionLog = await _exceptionLogRepository.UpdateRangeAsync(exceptionLog);

        return updatedExceptionLog;
    }

    public async Task<ExceptionLog> DeleteAsync(ExceptionLog exceptionLog, bool permanent = false)
    {
        ExceptionLog deletedExceptionLog = await _exceptionLogRepository.DeleteAsync(exceptionLog);

        return deletedExceptionLog;
    }
}
