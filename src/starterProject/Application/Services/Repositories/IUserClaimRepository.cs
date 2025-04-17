using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IUserClaimRepository : IAsyncRepository<UserClaim, Guid>, IRepository<UserClaim, Guid>
{
    Task<IList<SecurityClaim>> GetOperationClaimsByUserIdAsync(Guid userId);
}
