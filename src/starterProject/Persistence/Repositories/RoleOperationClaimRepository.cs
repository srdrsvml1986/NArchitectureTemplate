using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class RoleOperationClaimRepository : EfRepositoryBase<RoleOperationClaim, int, BaseDbContext>, IRoleOperationClaimRepository
{
    public RoleOperationClaimRepository(BaseDbContext context) : base(context)
    {
    }
}