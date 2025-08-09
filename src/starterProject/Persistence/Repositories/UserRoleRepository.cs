using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NArchitectureTemplate.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class UserRoleRepository : EfRepositoryBase<UserRole, int, BaseDbContext>, IUserRoleRepository
{
    public UserRoleRepository(BaseDbContext context) : base(context)
    {
    }
    public async Task<IList<Role>> GetSecurityRolesByUserIdAsync(Guid userId)
    {
        List<Role> operationClaims = await Query()
            .AsNoTracking()
            .Where(p => p.UserId.Equals(userId))
            .Select(p => new Role { Description = p.Role.Description, Name = p.Role.Name })
            .ToListAsync();
        return operationClaims;
    }
}