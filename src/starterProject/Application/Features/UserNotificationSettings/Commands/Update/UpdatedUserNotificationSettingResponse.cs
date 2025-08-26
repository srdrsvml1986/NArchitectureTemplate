using NArchitectureTemplate.Core.Application.Responses;
using Domain.Entities;

namespace Application.Features.UserNotificationSettings.Commands.Update;

public class UpdatedUserNotificationSettingResponse : IResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public NotificationType NotificationType { get; set; }
    public NotificationChannel NotificationChannel { get; set; }
    public bool IsEnabled { get; set; }
}