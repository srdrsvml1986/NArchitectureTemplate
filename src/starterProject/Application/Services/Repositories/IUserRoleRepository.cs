using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IUserRoleRepository : IAsyncRepository<UserRole, int>, IRepository<UserRole, int>
{
    Task<IList<Role>> GetSecurityRolesByUserIdAsync(Guid userId);

}