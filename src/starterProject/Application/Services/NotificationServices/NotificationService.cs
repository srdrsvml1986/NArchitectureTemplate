using Application.Services.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;
using NArchitectureTemplate.Core.Mailing;
using NArchitectureTemplate.Core.Notification.Services;
using NArchitectureTemplate.Core.Security.Entities;

namespace Application.Services.NotificationServices;

public class NotificationService : INotificationService
{
    private readonly IMailService _mailService;
    private readonly IUserRepository _userRepository;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly FrontedConfig _frontedConfig;

    public NotificationService(
        IUserRepository userRepository,
        IMailService mailService,
        IHostEnvironment hostEnvironment,
        IOptions<FrontedConfig> frontedConfig)
    {
        _userRepository = userRepository;
        _mailService = mailService;
        _hostEnvironment = hostEnvironment;
        _frontedConfig = frontedConfig.Value;
    }

    public async Task NotifySuspiciousSessionAsync(UserSession<Guid, Guid> session)
    {
        // Kullanıcı bilgilerini al
        var user = await _userRepository.GetAsync(u => u.Id == session.UserId);
        if (user == null) return;

        string baseUrl = _hostEnvironment.IsDevelopment()
            ? _frontedConfig.Development
            : _frontedConfig.Production;

        // Modern e-posta şablonu
        var subject = "🔒 Şüpheli Oturum Tespit Edildi";
        var htmlBody = $@"
        <!DOCTYPE html>
        <html lang=""tr"">
        <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Şüpheli Oturum Bildirimi</title>
            <style>
                /* Stil kodları aynı kalacak */
            </style>
        </head>
        <body>
            <div class=""container"">
                <div class=""header"">
                    <h1>🔒 Güvenlik Uyarısı</h1>
                </div>
                <div class=""content"">
                    <p class=""greeting"">Merhaba {user.FullName},</p>
                    
                    <p class=""message"">
                        Hesabınızda şüpheli bir oturum tespit edildi. 
                        Eğer bu işlemi siz yapmadıysanız, lütfen hesap güvenliğinizi sağlamak için 
                        aşağıdaki bilgileri inceleyin ve gerekli önlemleri alın.
                    </p>
                    
                    <div class=""alert-icon"">
                        ⚠️
                    </div>
                    
                    <div class=""details-card"">
                        <div class=""detail-item"">
                            <span class=""detail-label"">IP Adresi:</span>
                            <span class=""detail-value"">{session.IpAddress}</span>
                        </div>
                        <div class=""detail-item"">
                            <span class=""detail-label"">Lokasyon:</span>
                            <span class=""detail-value"">{session.LocationInfo}</span>
                        </div>
                        <div class=""detail-item"">
                            <span class=""detail-label"">Zaman:</span>
                            <span class=""detail-value"">{session.LoginTime:dd.MM.yyyy HH:mm}</span>
                        </div>
                    </div>
                    
                    <div class=""security-tips"">
                        <strong>🔐 Güvenlik Önerileri:</strong>
                        <ul>
                            <li>Hemen şifrenizi değiştirin</li>
                            <li>İki faktörlü kimlik doğrulamayı etkinleştirin</li>
                            <li>Hesabınızda tanımadığınız cihazları kontrol edin</li>
                            <li>Şüpheli durumlarda destek ekibimizle iletişime geçin</li>
                        </ul>
                    </div>
                    
                    <div class=""action-buttons"">
                        <a href=""{baseUrl}/change-password"" class=""button"">Şifremi Değiştir</a>
                        <a href=""{baseUrl}/security-settings"" class=""button"" style=""background: linear-gradient(135deg, #4CAF50 0%, #2E7D32 100%);"">Güvenlik Ayarları</a>
                    </div>
                    
                    <div class=""support-note"">
                        Sorularınız için <a href=""{baseUrl}/contact"" style=""color: #ff6b6b;"">destek ekibimizle</a> iletişime geçebilirsiniz.
                    </div>
                </div>
                <div class=""footer"">
                    <p>© {DateTime.Now.Year} Şirket Adı. Tüm hakları saklıdır.</p>
                    <p>Bu e-posta otomatik olarak gönderilmiştir, lütfen yanıtlamayın.</p>
                </div>
            </div>
        </body>
        </html>";

        await _mailService.SendEmailAsync(new Mail
        {
            ToList = new List<MailboxAddress> { new(name: user.FullName, user.Email) },
            Subject = subject,
            HtmlBody = htmlBody
        });
    }
}