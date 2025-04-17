using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class UserOperationClaimRepository
    : EfRepositoryBase<UserClaim, Guid, BaseDbContext>,
        IUserClaimRepository
{
    public UserOperationClaimRepository(BaseDbContext context)
        : base(context) { }

    public async Task<IList<SecurityClaim>> GetOperationClaimsByUserIdAsync(Guid userId)
    {
        List<SecurityClaim> operationClaims = await Query()
            .AsNoTracking()
            .Where(p => p.UserId.Equals(userId))
            .Select(p => new SecurityClaim { Id = p.ClaimId, Name = p.Claim.Name })
            .ToListAsync();
        return operationClaims;
    }
}
