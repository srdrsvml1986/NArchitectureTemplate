using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class RoleRepository : EfRepositoryBase<Role, int, BaseDbContext>, IRoleRepository
{
    public RoleRepository(BaseDbContext context) : base(context)
    {
    }
}