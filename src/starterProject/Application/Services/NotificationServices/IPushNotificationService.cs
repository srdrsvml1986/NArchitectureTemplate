using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.NotificationServices
{
    public interface IPushNotificationService
    {
        Task<PushNotificationResponse> SendAsync(PushNotification notification);
        Task<PushNotificationResponse> SendToTopicAsync(string topic, PushNotification notification);
        Task<bool> SubscribeToTopicAsync(string topic, List<string> deviceTokens);
        Task<bool> UnsubscribeFromTopicAsync(string topic, List<string> deviceTokens);
    }
}