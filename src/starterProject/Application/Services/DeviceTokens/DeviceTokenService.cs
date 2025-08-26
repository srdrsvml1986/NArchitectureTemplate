using Application.Features.DeviceTokens.Rules;
using Application.Services.Repositories;
using NArchitectureTemplate.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.DeviceTokens;

public class DeviceTokenService : IDeviceTokenService
{
    private readonly IDeviceTokenRepository _deviceTokenRepository;
    private readonly DeviceTokenBusinessRules _deviceTokenBusinessRules;

    public DeviceTokenService(IDeviceTokenRepository deviceTokenRepository, DeviceTokenBusinessRules deviceTokenBusinessRules)
    {
        _deviceTokenRepository = deviceTokenRepository;
        _deviceTokenBusinessRules = deviceTokenBusinessRules;
    }

    public async Task<DeviceToken?> GetAsync(
        Expression<Func<DeviceToken, bool>> predicate,
        Func<IQueryable<DeviceToken>, IIncludableQueryable<DeviceToken, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        DeviceToken? deviceToken = await _deviceTokenRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return deviceToken;
    }

    public async Task<IPaginate<DeviceToken>?> GetListAsync(
        Expression<Func<DeviceToken, bool>>? predicate = null,
        Func<IQueryable<DeviceToken>, IOrderedQueryable<DeviceToken>>? orderBy = null,
        Func<IQueryable<DeviceToken>, IIncludableQueryable<DeviceToken, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<DeviceToken> deviceTokenList = await _deviceTokenRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return deviceTokenList;
    }

    public async Task<DeviceToken> AddAsync(DeviceToken deviceToken, bool enableTracking = true)
    {
        var existingToken = await _deviceTokenRepository.GetByTokenAsync(deviceToken.Token);
        if (existingToken != null)
        {
            // Token zaten varsa güncelle
            existingToken.IsActive = true;
            existingToken.UpdatedAt = DateTime.UtcNow;
            existingToken.DeviceType = deviceToken.DeviceType;
            existingToken.DeviceName = deviceToken.DeviceName;
            return await _deviceTokenRepository.UpdateAsync(existingToken);
        }

        return await _deviceTokenRepository.AddAsync(deviceToken);
    }

    public async Task<ICollection<DeviceToken>> AddRangeAsync(ICollection<DeviceToken> deviceToken, bool enableTracking = true)
    {
        ICollection<DeviceToken> addedDeviceToken = await _deviceTokenRepository.AddRangeAsync(deviceToken);

        return addedDeviceToken;
    }

    public async Task<DeviceToken> UpdateAsync(DeviceToken deviceToken, bool enableTracking = true)
    {
        DeviceToken updatedDeviceToken = await _deviceTokenRepository.UpdateAsync(deviceToken);

        return updatedDeviceToken;
    }

    public async Task<ICollection<DeviceToken>> UpdateRangeAsync(ICollection<DeviceToken> deviceToken, bool enableTracking = true)
    {
        ICollection<DeviceToken> updatedDeviceToken = await _deviceTokenRepository.UpdateRangeAsync(deviceToken);

        return updatedDeviceToken;
    }

    public async Task RemoveWithTokenAsync(string token, bool permanent = false)
    {
        var deviceToken = await _deviceTokenRepository.GetByTokenAsync(token);
        if (deviceToken != null)
        {
            await _deviceTokenRepository.DeleteAsync(deviceToken);
        }
    }

    public async Task<DeviceToken> DeleteAsync(DeviceToken deviceToken, bool permanent = false)
    {
        DeviceToken deletedDeviceToken = await _deviceTokenRepository.DeleteAsync(deviceToken);

        return deletedDeviceToken;
    }

    public async Task<List<string>> GetUserDeviceTokensAsync(Guid userId)
    {
        var tokens = await _deviceTokenRepository.GetActiveTokensByUserIdAsync(userId);
        return tokens.Select(t => t.Token).ToList();
    }

    public async Task DeactivateTokenAsync(string token)
    {
        var deviceToken = await _deviceTokenRepository.GetByTokenAsync(token);
        if (deviceToken != null)
        {
            deviceToken.IsActive = false;
            deviceToken.UpdatedAt = DateTime.UtcNow;
            await _deviceTokenRepository.UpdateAsync(deviceToken);
        }
    }

    public async Task UpdateTokenAsync(DeviceToken deviceToken)
    {
        await _deviceTokenRepository.UpdateAsync(deviceToken);
    }
}
