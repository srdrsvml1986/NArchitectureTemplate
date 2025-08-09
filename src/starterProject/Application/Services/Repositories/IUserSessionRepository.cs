using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Repositories;
using System.Linq.Expressions;

namespace Application.Services.Repositories;

public interface IUserSessionRepository : IAsyncRepository<UserSession, Guid>, IRepository<UserSession, Guid>
{
   
}