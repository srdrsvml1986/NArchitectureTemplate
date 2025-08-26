using NArchitectureTemplate.Core.Application.Dtos;
using Domain.Entities;

namespace Application.Features.UserNotificationSettings.Queries.GetList;

public class GetListUserNotificationSettingListItemDto : IDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public NotificationType NotificationType { get; set; }
    public NotificationChannel NotificationChannel { get; set; }
    public bool IsEnabled { get; set; }
}