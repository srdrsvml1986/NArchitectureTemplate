using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IUserNotificationSettingRepository : IAsyncRepository<UserNotificationSetting, Guid>, IRepository<UserNotificationSetting, Guid>
{
}