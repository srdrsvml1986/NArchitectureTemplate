using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IGroupClaimRepository : IAsyncRepository<GroupClaim, int>, IRepository<GroupClaim, int>
{
}