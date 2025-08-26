using Application.Services.Repositories;
using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class UserNotificationSettingRepository : EfRepositoryBase<UserNotificationSetting, Guid, BaseDbContext>, IUserNotificationSettingRepository
{
    public UserNotificationSettingRepository(BaseDbContext context) : base(context)
    {
    }
}