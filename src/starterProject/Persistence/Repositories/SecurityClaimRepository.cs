using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class SecurityClaimRepository : EfRepositoryBase<Claim, int, BaseDbContext>, IClaimRepository
{
    public SecurityClaimRepository(BaseDbContext context)
        : base(context) { }
}
