using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface ILogRepository : IAsyncRepository<Log, Guid>, IRepository<Log, Guid>
{
}