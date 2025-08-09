using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NArchitectureTemplate.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class UserGroupRepository : EfRepositoryBase<UserGroup, int, BaseDbContext>, IUserGroupRepository
{
    public UserGroupRepository(BaseDbContext context) : base(context)
    {
    }
    public async Task<IList<Group>> GetSecurityGroupsByUserIdAsync(Guid userId)
    {
        List<Group> groups = await Query()
            .AsNoTracking()
            .Where(p => p.UserId.Equals(userId))
            .Select(p => new Group { Description = p.Group.Description, Name = p.Group.Name })
            .ToListAsync();
        return groups;
    }
}