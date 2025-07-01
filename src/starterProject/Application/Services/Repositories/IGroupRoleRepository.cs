using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IGroupRoleRepository : IAsyncRepository<GroupRole, int>, IRepository<GroupRole, int>
{
}