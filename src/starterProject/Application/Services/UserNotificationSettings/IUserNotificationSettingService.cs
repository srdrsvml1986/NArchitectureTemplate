using NArchitectureTemplate.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.UserNotificationSettings;

public interface IUserNotificationSettingService
{
    Task<UserNotificationSetting?> GetAsync(
        Expression<Func<UserNotificationSetting, bool>> predicate,
        Func<IQueryable<UserNotificationSetting>, IIncludableQueryable<UserNotificationSetting, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<UserNotificationSetting>?> GetListAsync(
        Expression<Func<UserNotificationSetting, bool>>? predicate = null,
        Func<IQueryable<UserNotificationSetting>, IOrderedQueryable<UserNotificationSetting>>? orderBy = null,
        Func<IQueryable<UserNotificationSetting>, IIncludableQueryable<UserNotificationSetting, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<UserNotificationSetting> AddAsync(UserNotificationSetting userNotificationSetting, bool enableTracking = true);
    Task<ICollection<UserNotificationSetting>> AddRangeAsync(ICollection<UserNotificationSetting> userNotificationSetting, bool enableTracking = true);
    Task<UserNotificationSetting> UpdateAsync(UserNotificationSetting userNotificationSetting, bool enableTracking = true);
    Task<ICollection<UserNotificationSetting>> UpdateRangeAsync(ICollection<UserNotificationSetting> userNotificationSetting, bool enableTracking = true);
    Task<UserNotificationSetting> DeleteAsync(UserNotificationSetting userNotificationSetting, bool permanent = false);
}
