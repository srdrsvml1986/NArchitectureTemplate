using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IUserSessionRepository : IAsyncRepository<UserSession, Guid>, IRepository<UserSession, Guid>
{
}