using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using NArchitectureTemplate.Core.Persistence.Paging;
using System.Linq.Expressions;

namespace Application.Services.DeviceTokens;

public interface IDeviceTokenService
{
    Task<DeviceToken?> GetAsync(
        Expression<Func<DeviceToken, bool>> predicate,
        Func<IQueryable<DeviceToken>, IIncludableQueryable<DeviceToken, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<DeviceToken>?> GetListAsync(
        Expression<Func<DeviceToken, bool>>? predicate = null,
        Func<IQueryable<DeviceToken>, IOrderedQueryable<DeviceToken>>? orderBy = null,
        Func<IQueryable<DeviceToken>, IIncludableQueryable<DeviceToken, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<DeviceToken> AddAsync(DeviceToken deviceToken, bool enableTracking = true);
    Task<ICollection<DeviceToken>> AddRangeAsync(ICollection<DeviceToken> deviceToken, bool enableTracking = true);
    Task<DeviceToken> UpdateAsync(DeviceToken deviceToken, bool enableTracking = true);
    Task<ICollection<DeviceToken>> UpdateRangeAsync(ICollection<DeviceToken> deviceToken, bool enableTracking = true);
    Task RemoveWithTokenAsync(string token, bool permanent = false);
    Task<DeviceToken> DeleteAsync(DeviceToken deviceToken, bool permanent = false);
    Task<List<string>> GetUserDeviceTokensAsync(Guid userId);
    Task DeactivateTokenAsync(string token);
    Task UpdateTokenAsync(DeviceToken deviceToken);
}
