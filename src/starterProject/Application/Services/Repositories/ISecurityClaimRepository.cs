using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface ISecurityClaimRepository : IAsyncRepository<SecurityClaim, int>, IRepository<SecurityClaim, int> { }
