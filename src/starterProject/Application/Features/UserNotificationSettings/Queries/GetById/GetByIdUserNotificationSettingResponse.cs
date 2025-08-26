using Domain.Entities;
using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.UserNotificationSettings.Queries.GetById;

public class GetByIdUserNotificationSettingResponse : IResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public NotificationType NotificationType { get; set; }
    public NotificationChannel NotificationChannel { get; set; }
    public bool IsEnabled { get; set; }
}