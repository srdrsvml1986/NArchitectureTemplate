using System.Collections.Generic;

namespace NArchitectureTemplate.Core.Notification.Services
{
    public enum PushNotificationPriority
    {
        Low,
        Normal,
        High
    }

    public enum PushPlatform
    {
        Android,
        iOS,
        Web,
        All
    }

    public class PushNotification
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
        public PushNotificationPriority Priority { get; set; } = PushNotificationPriority.Normal;
        public PushPlatform Platform { get; set; } = PushPlatform.All;
        public List<string> DeviceTokens { get; set; } = new List<string>();
        public string Topic { get; set; }
        public string ImageUrl { get; set; }
        public int TimeToLive { get; set; } = 3600; // in seconds
    }

    public class PushNotificationResponse
    {
        public bool IsSuccess { get; set; }
        public string MessageId { get; set; }
        public string ErrorMessage { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<string> FailedDeviceTokens { get; set; }
    }
}