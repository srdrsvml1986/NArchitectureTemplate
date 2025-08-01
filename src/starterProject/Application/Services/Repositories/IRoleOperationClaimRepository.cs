using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IRoleOperationClaimRepository : IAsyncRepository<RoleOperationClaim, int>, IRepository<RoleOperationClaim, int>
{
}