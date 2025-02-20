using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class OperationClaimRepository : EfRepositoryBase<Claim, int, BaseDbContext>, IClaimRepository
{
    public OperationClaimRepository(BaseDbContext context)
        : base(context) { }
}
