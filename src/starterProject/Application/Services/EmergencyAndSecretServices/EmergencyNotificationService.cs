using Application.Services.NotificationServices;
using Microsoft.Extensions.Configuration;
using MimeKit;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction;
using NArchitectureTemplate.Core.Mailing;

namespace Application.Services.EmergencyAndSecretServices;

public interface IEmergencyNotifier
{
    Task NotifySecurityBreachAsync(string breachType, string details, string severity = "HIGH");
    Task NotifySystemFailureAsync(string component, string errorDetails, string severity = "CRITICAL");
    Task NotifyDataBreachAsync(string dataType, string scope, string severity = "HIGH");
    Task NotifyBackupStatusAsync(string status, string details, string severity = "MEDIUM");
    Task NotifyKeyRotationAsync(string keyType, string operation, string severity = "MEDIUM");
}

public class EmergencyNotificationService : IEmergencyNotifier
{
    private readonly IConfiguration _configuration;
    private readonly IMailService _mailService;
    private readonly ILogger _logger;
    private readonly ISmsService _smsService;
    private readonly IPushNotificationService _pushNotificationService;

    public EmergencyNotificationService(
        IConfiguration configuration,
        ILogger logger,
        IMailService mailService,
        ISmsService smsService,
        IPushNotificationService pushNotificationService)
    {
        _configuration = configuration;
        _logger = logger;
        _mailService = mailService;
        _smsService = smsService;
        _pushNotificationService = pushNotificationService;
    }

    public async Task NotifySecurityBreachAsync(string breachType, string details, string severity = "HIGH")
    {
        var message = $"GÜVENLİK İHLALİ - {breachType}: {details}";
        await SendEmergencyNotificationAsync("SECURITY_BREACH", message, severity);
    }

    public async Task NotifySystemFailureAsync(string component, string errorDetails, string severity = "CRITICAL")
    {
        var message = $"SİSTEM HATASI - {component}: {errorDetails}";
        await SendEmergencyNotificationAsync("SYSTEM_FAILURE", message, severity);
    }

    public async Task NotifyDataBreachAsync(string dataType, string scope, string severity = "HIGH")
    {
        var message = $"VERİ İHLALİ - {dataType} verisi etkilendi: {scope}";
        await SendEmergencyNotificationAsync("DATA_BREACH", message, severity);
    }

    public async Task NotifyBackupStatusAsync(string status, string details, string severity = "MEDIUM")
    {
        var message = $"YEDEKLEME DURUMU - {status}: {details}";
        await SendEmergencyNotificationAsync("BACKUP_STATUS", message, severity);
    }

    public async Task NotifyKeyRotationAsync(string keyType, string operation, string severity = "MEDIUM")
    {
        var message = $"ANAHTAR YÖNETİMİ - {keyType} için {operation} işlemi";
        await SendEmergencyNotificationAsync("KEY_ROTATION", message, severity);
    }

    private async Task SendEmergencyNotificationAsync(string alertType, string message, string severity)
    {
        var recipients = GetEmergencyRecipients(severity);

        foreach (var recipient in recipients)
        {
            try
            {
                await SendMultiChannelAlertAsync(recipient, alertType, message, severity);
            }
            catch (Exception ex)
            {
                _logger.Error(ex,String.Format("Acil durum bildirimi gönderilemedi: {Recipient}", recipient)+" "+ ex.Message);
            }
        }
    }

    private List<EmergencyContact> GetEmergencyRecipients(string severity)
    {
        var recipients = new List<EmergencyContact>();

        // Yapılandırmadan alıcıları al
        var configRecipients = _configuration["Emergency:NotificationRecipients"]?.Split(',');
        if (configRecipients != null)
        {
            foreach (var recipient in configRecipients)
            {
                var contact = ParseContact(recipient.Trim());
                if (contact != null && ShouldNotify(contact, severity))
                {
                    recipients.Add(contact);
                }
            }
        }

        return recipients;
    }

    private EmergencyContact ParseContact(string contactInfo)
    {
        if (contactInfo.Contains("@"))
            return new EmergencyContact { Email = contactInfo, Type = ContactType.Email };

        if (IsPhoneNumber(contactInfo))
            return new EmergencyContact { Phone = contactInfo, Type = ContactType.Phone };

        // Diğer iletişim türleri (slack, teams, vs.) için parsing
        return null;
    }

    private bool ShouldNotify(EmergencyContact contact, string severity)
    {
        // Önem derecesine göre filtreleme
        return severity switch
        {
            "CRITICAL" => contact.ReceiveCriticalAlerts,
            "HIGH" => contact.ReceiveHighAlerts,
            "MEDIUM" => contact.ReceiveMediumAlerts,
            "LOW" => contact.ReceiveLowAlerts,
            _ => true
        };
    }

    private async Task SendMultiChannelAlertAsync(EmergencyContact contact, string alertType,
                                                string message, string severity)
    {
        var notificationTasks = new List<Task>();

        // Çoklu kanal bildirimi
        if (!string.IsNullOrEmpty(contact.Email))
            notificationTasks.Add(SendEmergencyEmailAsync(contact.Email, alertType, message, severity));

        if (!string.IsNullOrEmpty(contact.Phone))
            notificationTasks.Add(SendEmergencySmsAsync(contact.Phone, alertType, message, severity));

        if (!string.IsNullOrEmpty(contact.SlackWebhook))
            notificationTasks.Add(SendSlackAlertAsync(contact.SlackWebhook, alertType, message, severity));

        if (!string.IsNullOrEmpty(contact.TeamsWebhook))
            notificationTasks.Add(SendTeamsAlertAsync(contact.TeamsWebhook, alertType, message, severity));

        await Task.WhenAll(notificationTasks);
    }

    private async Task SendEmergencyEmailAsync(string email, string alertType, string message, string severity)
    {
        var subject = $"[{severity}] ACİL DURUM: {alertType}";
        var body = $@"
            ACİL DURUM BİLDİRİMİ
            --------------------
            Tip: {alertType}
            Önem Derecesi: {severity}
            Zaman: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC
            Mesaj: {message}
            
            Bu bir otomatik acil durum bildirimidir.
            Lütfen derhal harekete geçin.
        ";

        await _mailService.SendEmailAsync(new Mail
        {
            ToList = new List<MailboxAddress> { new("", email) },
            Subject = subject,
            TextBody = body,
            Priority = GetMailPriority(severity)
        });
    }

    private async Task SendEmergencySmsAsync(string phoneNumber, string alertType, string message, string severity)
    {
        var smsMessage = new SmsMessage
        {
            To = phoneNumber,
            Content = $"ACİL: {alertType} - {TruncateMessage(message, 140)}",
            Priority = (SmsPriority)GetSmsPriority(severity),
            ReferenceId = $"emergency_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}",
            Sender = _configuration["Emergency:SmsSender"] ?? "EMERGENCY",
            PreferredProvider = GetProviderPreference(severity),
            Metadata = new Dictionary<string, string>
        {
            { "emergency_type", alertType },
            { "severity", severity },
            { "timestamp", DateTime.UtcNow.ToString("O") }
        }
        };

        var result = await _smsService.SendAsync(smsMessage);

        if (!result.IsSuccess)
        {
            _logger.Critical(String.Format("Emergency SMS failed to {Phone} via {Provider}: {Error}",
                phoneNumber, result.Provider, result.ErrorMessage));

            // Fallback otomatik olarak yapıldı, ekstra loglama
            if (result.IsFallback)
            {
                _logger.Information(String.Format("Emergency SMS delivered via fallback provider: {Provider}",
                    result.Provider));
            }
        }
        else
        {
            _logger.Information(String.Format("Emergency SMS sent successfully to {Phone} via {Provider}",
                phoneNumber, result.Provider));
        }
    }

    private SmsProvider GetProviderPreference(string severity)
    {
        // Önem derecesine göre sağlayıcı tercihi
        return severity switch
        {
            "CRITICAL" => SmsProvider.Turkcell, // Turkcell daha güvenilir olabilir
            "HIGH" => SmsProvider.Turkcell,
            _ => SmsProvider.Vodafone // Varsayılan
        };
    }

    private async Task SendSlackAlertAsync(string webhookUrl, string alertType, string message, string severity)
    {
        // Slack webhook entegrasyonu
        var slackMessage = new
        {
            text = $"🚨 *{alertType}* - _{severity}_\n{message}",
            color = GetSeverityColor(severity)
        };

        // HTTP client ile webhook'a POST isteği
    }

    private async Task SendTeamsAlertAsync(string webhookUrl, string alertType, string message, string severity)
    {
        // Microsoft Teams webhook entegrasyonu
        var teamsMessage = new
        {
            title = $"ACİL DURUM: {alertType}",
            text = message,
            themeColor = GetSeverityColor(severity)
        };

        // HTTP client ile webhook'a POST isteği
    }

    private string TruncateMessage(string message, int maxLength)
    {
        return message.Length <= maxLength ? message : message.Substring(0, maxLength - 3) + "...";
    }

    private MailPriority GetMailPriority(string severity)
    {
        return severity switch
        {
            "CRITICAL" => MailPriority.High,
            "HIGH" => MailPriority.High,
            _ => MailPriority.Normal
        };
    }

    private int GetSmsPriority(string severity)
    {
        return severity switch
        {
            "CRITICAL" => 1,
            "HIGH" => 2,
            "MEDIUM" => 3,
            _ => 4
        };
    }

    private string GetSeverityColor(string severity)
    {
        return severity switch
        {
            "CRITICAL" => "#FF0000",
            "HIGH" => "#FF3300",
            "MEDIUM" => "#FF9900",
            "LOW" => "#FFFF00",
            _ => "#CCCCCC"
        };
    }

    private bool IsPhoneNumber(string input)
    {
        // Basit telefon numarası validasyonu
        return System.Text.RegularExpressions.Regex.IsMatch(input, @"^\+?[1-9]\d{1,14}$");
    }
}

// Yardımcı sınıflar
public class EmergencyContact
{
    public string Email { get; set; }
    public string Phone { get; set; }
    public string SlackWebhook { get; set; }
    public string TeamsWebhook { get; set; }
    public ContactType Type { get; set; }
    public bool ReceiveCriticalAlerts { get; set; } = true;
    public bool ReceiveHighAlerts { get; set; } = true;
    public bool ReceiveMediumAlerts { get; set; } = false;
    public bool ReceiveLowAlerts { get; set; } = false;
}

public enum ContactType
{
    Email,
    Phone,
    Slack,
    Teams,
    Other
}

public interface IPushNotificationService
{
    Task SendPushNotificationAsync(string deviceId, string title, string message, string severity);
}