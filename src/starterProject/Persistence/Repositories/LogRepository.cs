using Application.Services.Repositories;
using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class LogRepository : EfRepositoryBase<Log, Guid, BaseDbContext>, ILogRepository
{
    public LogRepository(BaseDbContext context) : base(context)
    {
    }
}