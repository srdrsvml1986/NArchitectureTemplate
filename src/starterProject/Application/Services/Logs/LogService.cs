using Application.Features.Logs.Rules;
using Application.Services.Repositories;
using NArchitectureTemplate.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.Logs;

public class LogService : ILogService
{
    private readonly ILogRepository _logRepository;
    private readonly LogBusinessRules _logBusinessRules;

    public LogService(ILogRepository logRepository, LogBusinessRules logBusinessRules)
    {
        _logRepository = logRepository;
        _logBusinessRules = logBusinessRules;
    }

    public async Task<Log?> GetAsync(
        Expression<Func<Log, bool>> predicate,
        Func<IQueryable<Log>, IIncludableQueryable<Log, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        Log? log = await _logRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return log;
    }

    public async Task<IPaginate<Log>?> GetListAsync(
        Expression<Func<Log, bool>>? predicate = null,
        Func<IQueryable<Log>, IOrderedQueryable<Log>>? orderBy = null,
        Func<IQueryable<Log>, IIncludableQueryable<Log, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<Log> logList = await _logRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return logList;
    }

    public async Task<Log> AddAsync(Log log, bool enableTracking = true)
    {
        Log addedLog = await _logRepository.AddAsync(log);

        return addedLog;
    }

    public async Task<ICollection<Log>> AddRangeAsync(ICollection<Log> log, bool enableTracking = true)
    {
        ICollection<Log> addedLog = await _logRepository.AddRangeAsync(log);

        return addedLog;
    }

    public async Task<Log> UpdateAsync(Log log, bool enableTracking = true)
    {
        Log updatedLog = await _logRepository.UpdateAsync(log);

        return updatedLog;
    }

    public async Task<ICollection<Log>> UpdateRangeAsync(ICollection<Log> log, bool enableTracking = true)
    {
        ICollection<Log> updatedLog = await _logRepository.UpdateRangeAsync(log);

        return updatedLog;
    }

    public async Task<Log> DeleteAsync(Log log, bool permanent = false)
    {
        Log deletedLog = await _logRepository.DeleteAsync(log);

        return deletedLog;
    }
}
