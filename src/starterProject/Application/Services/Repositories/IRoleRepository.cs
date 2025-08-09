using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IRoleRepository : IAsyncRepository<Role, int>, IRepository<Role, int>
{
}