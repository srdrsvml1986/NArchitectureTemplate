using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class UserSessionRepository : EfRepositoryBase<UserSession, Guid, BaseDbContext>, IUserSessionRepository
{
    public UserSessionRepository(BaseDbContext context) : base(context)
    {
    }
}