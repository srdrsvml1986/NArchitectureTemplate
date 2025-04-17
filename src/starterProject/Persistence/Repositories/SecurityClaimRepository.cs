using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class SecurityClaimRepository : EfRepositoryBase<SecurityClaim, int, BaseDbContext>, ISecurityClaimRepository
{
    public SecurityClaimRepository(BaseDbContext context)
        : base(context) { }
}
