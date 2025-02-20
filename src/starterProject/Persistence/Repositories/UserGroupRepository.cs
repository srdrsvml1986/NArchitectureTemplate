using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class UserGroupRepository : EfRepositoryBase<UserGroup, int, BaseDbContext>, IUserGroupRepository
{
    public UserGroupRepository(BaseDbContext context) : base(context)
    {
    }
}