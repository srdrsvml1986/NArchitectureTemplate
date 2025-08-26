using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NArchitectureTemplate.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class DeviceTokenRepository : EfRepositoryBase<DeviceToken, Guid, BaseDbContext>, IDeviceTokenRepository
{
    public DeviceTokenRepository(BaseDbContext context) : base(context)
    {
    }

    public async Task<List<DeviceToken>> GetByUserIdAsync(Guid userId)
    {
        return await Query().Where(dt => dt.UserId == userId).ToListAsync();
    }

    public async Task<DeviceToken> GetByTokenAsync(string token)
    {
        return await Query().FirstOrDefaultAsync(dt => dt.Token == token);
    }

    public async Task<List<DeviceToken>> GetActiveTokensByUserIdAsync(Guid userId)
    {
        return await Query().Where(dt => dt.UserId == userId && dt.IsActive).ToListAsync();
    }
}