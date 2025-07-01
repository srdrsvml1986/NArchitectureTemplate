using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IRoleClaimRepository : IAsyncRepository<RoleClaim, int>, IRepository<RoleClaim, int>
{
}