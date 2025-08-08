using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using System.Linq.Expressions;

namespace Application.Services.Repositories;

public interface IUserSessionRepository : IAsyncRepository<UserSession, Guid>, IRepository<UserSession, Guid>
{
   
}