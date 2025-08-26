using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Domain.Entities;

public class UserNotificationSetting: Entity<Guid>
{
    public required Guid UserId { get; set; }
    public virtual User User { get; set; }= default!;

    public required NotificationType NotificationType { get; set; }

    public required NotificationChannel NotificationChannel { get; set; }

    public bool IsEnabled { get; set; } = true;

}
public enum NotificationType
{
    Email,
    SMS,
    PushNotification,
    InApp
}

public enum NotificationChannel
{
    NewMessage,
    Promotion,
    SystemAlert,
    OrderStatus,
    SecurityAlert,
    CommentReply,
    NewFollower,
    PriceAlert,
    EventReminder
}