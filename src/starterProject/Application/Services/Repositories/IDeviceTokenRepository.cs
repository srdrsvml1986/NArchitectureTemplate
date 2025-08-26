using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IDeviceTokenRepository : IAsyncRepository<DeviceToken, Guid>, IRepository<DeviceToken, Guid>
{
    Task<List<DeviceToken>> GetByUserIdAsync(Guid userId);
    Task<DeviceToken> GetByTokenAsync(string token);
    Task<List<DeviceToken>> GetActiveTokensByUserIdAsync(Guid userId);
}