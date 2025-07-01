using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class GroupRoleRepository : EfRepositoryBase<GroupRole, int, BaseDbContext>, IGroupRoleRepository
{
    public GroupRoleRepository(BaseDbContext context) : base(context)
    {
    }
}