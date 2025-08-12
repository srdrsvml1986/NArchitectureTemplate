using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IExceptionLogRepository : IAsyncRepository<ExceptionLog, Guid>, IRepository<ExceptionLog, Guid>
{
}