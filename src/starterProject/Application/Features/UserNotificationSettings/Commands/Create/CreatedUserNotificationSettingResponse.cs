using NArchitectureTemplate.Core.Application.Responses;
using Domain.Entities;

namespace Application.Features.UserNotificationSettings.Commands.Create;

public class CreatedUserNotificationSettingResponse : IResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public NotificationType NotificationType { get; set; }
    public NotificationChannel NotificationChannel { get; set; }
    public bool IsEnabled { get; set; }
}