using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IGroupOperationClaimRepository : IAsyncRepository<GroupOperationClaim, int>, IRepository<GroupOperationClaim, int>
{
}