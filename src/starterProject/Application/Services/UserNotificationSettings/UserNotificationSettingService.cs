using Application.Features.UserNotificationSettings.Rules;
using Application.Services.Repositories;
using NArchitectureTemplate.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.UserNotificationSettings;

public class UserNotificationSettingService : IUserNotificationSettingService
{
    private readonly IUserNotificationSettingRepository _userNotificationSettingRepository;
    private readonly UserNotificationSettingBusinessRules _userNotificationSettingBusinessRules;

    public UserNotificationSettingService(IUserNotificationSettingRepository userNotificationSettingRepository, UserNotificationSettingBusinessRules userNotificationSettingBusinessRules)
    {
        _userNotificationSettingRepository = userNotificationSettingRepository;
        _userNotificationSettingBusinessRules = userNotificationSettingBusinessRules;
    }

    public async Task<UserNotificationSetting?> GetAsync(
        Expression<Func<UserNotificationSetting, bool>> predicate,
        Func<IQueryable<UserNotificationSetting>, IIncludableQueryable<UserNotificationSetting, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        UserNotificationSetting? userNotificationSetting = await _userNotificationSettingRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return userNotificationSetting;
    }

    public async Task<IPaginate<UserNotificationSetting>?> GetListAsync(
        Expression<Func<UserNotificationSetting, bool>>? predicate = null,
        Func<IQueryable<UserNotificationSetting>, IOrderedQueryable<UserNotificationSetting>>? orderBy = null,
        Func<IQueryable<UserNotificationSetting>, IIncludableQueryable<UserNotificationSetting, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<UserNotificationSetting> userNotificationSettingList = await _userNotificationSettingRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return userNotificationSettingList;
    }

    public async Task<UserNotificationSetting> AddAsync(UserNotificationSetting userNotificationSetting, bool enableTracking = true)
    {
        UserNotificationSetting addedUserNotificationSetting = await _userNotificationSettingRepository.AddAsync(userNotificationSetting);

        return addedUserNotificationSetting;
    }

    public async Task<ICollection<UserNotificationSetting>> AddRangeAsync(ICollection<UserNotificationSetting> userNotificationSetting, bool enableTracking = true)
    {
        ICollection<UserNotificationSetting> addedUserNotificationSetting = await _userNotificationSettingRepository.AddRangeAsync(userNotificationSetting);

        return addedUserNotificationSetting;
    }

    public async Task<UserNotificationSetting> UpdateAsync(UserNotificationSetting userNotificationSetting, bool enableTracking = true)
    {
        UserNotificationSetting updatedUserNotificationSetting = await _userNotificationSettingRepository.UpdateAsync(userNotificationSetting);

        return updatedUserNotificationSetting;
    }

    public async Task<ICollection<UserNotificationSetting>> UpdateRangeAsync(ICollection<UserNotificationSetting> userNotificationSetting, bool enableTracking = true)
    {
        ICollection<UserNotificationSetting> updatedUserNotificationSetting = await _userNotificationSettingRepository.UpdateRangeAsync(userNotificationSetting);

        return updatedUserNotificationSetting;
    }

    public async Task<UserNotificationSetting> DeleteAsync(UserNotificationSetting userNotificationSetting, bool permanent = false)
    {
        UserNotificationSetting deletedUserNotificationSetting = await _userNotificationSettingRepository.DeleteAsync(userNotificationSetting);

        return deletedUserNotificationSetting;
    }
}
