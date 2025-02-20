using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class GroupClaimRepository : EfRepositoryBase<GroupClaim, int, BaseDbContext>, IGroupClaimRepository
{
    public GroupClaimRepository(BaseDbContext context) : base(context)
    {
    }
}