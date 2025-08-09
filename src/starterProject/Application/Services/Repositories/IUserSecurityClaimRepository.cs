using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IUserOperationClaimRepository : IAsyncRepository<UserOperationClaim, Guid>, IRepository<UserOperationClaim, Guid>
{
    Task<IList<OperationClaim>> GetSecurityClaimsByUserIdAsync(Guid userId);
}
