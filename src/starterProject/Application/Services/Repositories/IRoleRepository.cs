using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IRoleRepository : IAsyncRepository<Role, int>, IRepository<Role, int>
{
}