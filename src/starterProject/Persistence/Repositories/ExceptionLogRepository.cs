using Application.Services.Repositories;
using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class ExceptionLogRepository : EfRepositoryBase<ExceptionLog, Guid, BaseDbContext>, IExceptionLogRepository
{
    public ExceptionLogRepository(BaseDbContext context) : base(context)
    {
    }
}