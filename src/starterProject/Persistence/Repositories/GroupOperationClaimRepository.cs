using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class GroupOperationClaimRepository : EfRepositoryBase<GroupOperationClaim, int, BaseDbContext>, IGroupOperationClaimRepository
{
    public GroupOperationClaimRepository(BaseDbContext context) : base(context)
    {
    }
}