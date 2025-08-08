using Application.Services.Repositories;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;
public interface INotificationService
{
    Task NotifySuspiciousSessionAsync(UserSession session);
}
public class NotificationService : INotificationService
{
    private readonly IEmailSender _emailSender;
    private readonly IUserRepository _userRepository;

    public NotificationService(IEmailSender emailSender, IUserRepository userRepository)
    {
        _emailSender = emailSender;
        _userRepository = userRepository;
    }

    public async Task NotifySuspiciousSessionAsync(UserSession session)
    {
        // Kullanıcı bilgilerini al
        var user = await _userRepository.GetAsync(u => u.Id == session.UserId);
        if (user == null) return;

        // E-posta gönderimi
        var subject = "Şüpheli Oturum Bildirimi";
        var body = $"Hesabınızdan şüpheli bir oturum algılandı:\n" +
            $"IP: {session.IpAddress}\n" +
            $"Lokasyon: {session.LocationInfo}\n" +
            $"Zaman: {session.LoginTime}";
        await _emailSender.SendEmailAsync(user.Email, subject, body);
    }
}
