using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IUserSecurityClaimRepository : IAsyncRepository<UserSecurityClaim, Guid>, IRepository<UserSecurityClaim, Guid>
{
    Task<IList<SecurityClaim>> GetOperationClaimsByUserIdAsync(Guid userId);
}
