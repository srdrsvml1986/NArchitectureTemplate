//using Domain.Entities;
//using global::Application.Services.Repositories;
//using Microsoft.AspNetCore.Identity.UI.Services;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Application.Services;

//public enum NotificationType
//{
//    Email,
//    Sms,
//    Push,
//    InApp
//}

//public enum NotificationEvent
//{
//    SuspiciousSession,
//    PasswordChanged,
//    AccountLocked,
//    NewLogin,
//    TwoFactorEnabled,
//    ProfileUpdated
//}

//public class NotificationMessage
//{
//    public string Subject { get; set; }
//    public string Body { get; set; }
//    public Dictionary<string, string> Metadata { get; set; } = new();
//}

//public interface INotificationService
//{
//    Task SendNotificationAsync(
//        NotificationEvent notificationEvent,
//        NotificationType notificationType,
//        string recipient,
//        NotificationMessage message);
//}

//public class NotificationService : INotificationService
//{
//    private readonly IEmailSender _emailSender;
//    private readonly IUserRepository _userRepository;
//    private readonly ISmsService _smsService; // Yeni SMS servisi
//    private readonly IPushNotificationService _pushService; // Yeni push servisi

//    public NotificationService(
//        IEmailSender emailSender,
//        IUserRepository userRepository,
//        ISmsService smsService,
//        IPushNotificationService pushService)
//    {
//        _emailSender = emailSender;
//        _userRepository = userRepository;
//        _smsService = smsService;
//        _pushService = pushService;
//    }

//    public async Task SendNotificationAsync(
//        NotificationEvent notificationEvent,
//        NotificationType notificationType,
//        string recipient,
//        NotificationMessage message)
//    {
//        switch (notificationType)
//        {
//            case NotificationType.Email:
//                await SendEmailNotification(recipient, message);
//                break;

//            case NotificationType.Sms:
//                await SendSmsNotification(recipient, message);
//                break;

//            case NotificationType.Push:
//                await SendPushNotification(recipient, message);
//                break;

//            case NotificationType.InApp:
//                await SendInAppNotification(recipient, message);
//                break;
//        }

//        await LogNotification(notificationEvent, notificationType, recipient);
//    }

//    private async Task SendEmailNotification(string email, NotificationMessage message)
//    {
//        await _emailSender.SendEmailAsync(email, message.Subject, message.Body);
//    }

//    private async Task SendSmsNotification(string phoneNumber, NotificationMessage message)
//    {
//        await _smsService.SendSmsAsync(phoneNumber, message.Body);
//    }

//    private async Task SendPushNotification(string userId, NotificationMessage message)
//    {
//        var user = await _userRepository.GetAsync(u => u.Id == userId);
//        if (user == null) return;

//        await _pushService.SendPushAsync(
//            user.DeviceToken,
//            message.Subject,
//            message.Body);
//    }

//    private async Task SendInAppNotification(string userId, NotificationMessage message)
//    {
//        await _userRepository.AddUserNotificationAsync(
//            userId,
//            message.Subject,
//            message.Body);
//    }

//    private async Task LogNotification(
//        NotificationEvent notificationEvent,
//        NotificationType notificationType,
//        string recipient)
//    {
//        // Bildirim kaydını veritabanına ekle
//        await _userRepository.AddNotificationLogAsync(
//            notificationEvent.ToString(),
//            notificationType.ToString(),
//            recipient);
//    }
//}