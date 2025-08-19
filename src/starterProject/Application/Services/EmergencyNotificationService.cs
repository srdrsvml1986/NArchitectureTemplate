using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services;
// EmergencyNotificationService.cs
public class EmergencyNotificationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmergencyNotificationService> _logger;

    public EmergencyNotificationService(
        IConfiguration configuration,
        ILogger<EmergencyNotificationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmergencyAlertAsync(string alertType, string message)
    {
        var recipients = _configuration["Emergency:NotificationRecipients"]?.Split(',');
        if (recipients == null || recipients.Length == 0)
            return;

        foreach (var recipient in recipients)
        {
            try
            {
                await SendAlertToRecipient(recipient.Trim(), alertType, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Acil durum bildirimi gönderilemedi: {Recipient}", recipient);
            }
        }
    }

    private async Task SendAlertToRecipient(string recipient, string alertType, string message)
    {
        // E-posta, SMS veya diğer bildirim yöntemleri
        if (recipient.Contains("@"))
        {
            await SendEmailAlert(recipient, alertType, message);
        }
        else
        {
            await SendSmsAlert(recipient, alertType, message);
        }
    }

    private async Task SendEmailAlert(string email, string alertType, string message)
    {
        // E-posta gönderme implementasyonu
        _logger.LogWarning("EMAIL_ALERT: {Email} | {AlertType} | {Message}",
            email, alertType, message);

        // Gerçek e-posta gönderme kodu buraya gelecek
        await Task.Delay(100); // Simülasyon
    }

    private async Task SendSmsAlert(string phoneNumber, string alertType, string message)
    {
        // SMS gönderme implementasyonu
        _logger.LogWarning("SMS_ALERT: {Phone} | {AlertType} | {Message}",
            phoneNumber, alertType, message);

        // Gerçek SMS gönderme kodu buraya gelecek
        await Task.Delay(100); // Simülasyon
    }
}