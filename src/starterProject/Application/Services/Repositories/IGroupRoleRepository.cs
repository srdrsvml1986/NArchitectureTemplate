using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IGroupRoleRepository : IAsyncRepository<GroupRole, int>, IRepository<GroupRole, int>
{
}