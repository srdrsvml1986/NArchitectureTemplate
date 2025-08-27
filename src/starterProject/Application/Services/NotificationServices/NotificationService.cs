using Application.Services.Repositories;
using MimeKit;
using NArchitectureTemplate.Core.Mailing;
using NArchitectureTemplate.Core.Notification.Services;
using NArchitectureTemplate.Core.Security.Entities;

namespace Application.Services.NotificationServices;

public class NotificationService : INotificationService
{
    private readonly IMailService _mailService;
    private readonly IUserRepository _userRepository;

    public NotificationService(IUserRepository userRepository, IMailService mailService)
    {
        _userRepository = userRepository;
        _mailService = mailService;
    }

    public async Task NotifySuspiciousSessionnAsync(UserSession session)
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

        await _mailService.SendEmailAsync(new Mail
        {
            ToList = new List<MailboxAddress> { new(name: user.FullName, user.Email) },
            Subject = subject,
            TextBody = body
        });
    }

    public async Task NotifySuspiciousSessionAsync(UserSession<Guid, Guid> session)
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

        await _mailService.SendEmailAsync(new Mail
        {
            ToList = new List<MailboxAddress> { new(name: user.FullName, user.Email) },
            Subject = subject,
            TextBody = body
        });
    }
}
