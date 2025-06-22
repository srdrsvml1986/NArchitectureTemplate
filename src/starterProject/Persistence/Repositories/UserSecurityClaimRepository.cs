using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class UserClaimRepository
    : EfRepositoryBase<UserClaim, Guid, BaseDbContext>,
        IUserClaimRepository
{
    public UserClaimRepository(BaseDbContext context)
        : base(context) { }

    public async Task<IList<SecurityClaim>> GetSecurityClaimsByUserIdAsync(Guid userId)
    {
        List<SecurityClaim> operationClaims = await Query()
            .AsNoTracking()
            .Where(p => p.UserId.Equals(userId))
            .Select(p => new SecurityClaim { Id = p.SecurityClaimId, Name = p.Claim.Name })
            .ToListAsync();
        return operationClaims;
    }
}
