using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class GroupRepository : EfRepositoryBase<Group, int, BaseDbContext>, IGroupRepository
{
    public GroupRepository(BaseDbContext context) : base(context)
    {
    }
}