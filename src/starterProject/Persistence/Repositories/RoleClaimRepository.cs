using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class RoleClaimRepository : EfRepositoryBase<RoleClaim, int, BaseDbContext>, IRoleClaimRepository
{
    public RoleClaimRepository(BaseDbContext context) : base(context)
    {
    }
}