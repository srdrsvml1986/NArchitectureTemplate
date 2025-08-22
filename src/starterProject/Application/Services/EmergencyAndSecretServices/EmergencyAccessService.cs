using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace Application.Services.EmergencyAndSecretServices;

public class EmergencyAccessService
{
    private readonly ILocalSecretsManager _secretsManager;
    private readonly IConfiguration _configuration;
    private readonly AuditService _auditService;
    private readonly ILogger<EmergencyAccessService> _logger;

    public EmergencyAccessService(
        ILocalSecretsManager secretsManager,
        IConfiguration configuration,
        AuditService auditService,
        ILogger<EmergencyAccessService> logger)
    {
        _secretsManager = secretsManager;
        _configuration = configuration;
        _auditService = auditService;
        _logger = logger;
    }

    public bool ValidateEmergencyToken(string token)
    {
        var emergencyToken = _configuration["Emergency:AccessToken"];
        if (string.IsNullOrEmpty(emergencyToken))
        {
            _logger.LogWarning("Acil durum tokenı konfigürasyonda tanımlanmamış");
            return false;
        }

        var validUntilStr = _configuration["Emergency:ValidUntil"];
        if (string.IsNullOrEmpty(validUntilStr) || !DateTime.TryParse(validUntilStr, out var validUntil))
        {
            _logger.LogWarning("Acil durum token geçerlilik süresi konfigürasyonda tanımlanmamış veya geçersiz");
            return false;
        }

        return token == emergencyToken && DateTime.UtcNow <= validUntil;
    }

    public Dictionary<string, string> GetSecretsForEmergency(string token, string requesterIp)
    {
        if (!ValidateEmergencyToken(token))
        {
            _auditService.LogAccess("ALL", "EMERGENCY_ACCESS_DENIED", "Unknown", requesterIp);
            throw new UnauthorizedAccessException("Geçersiz acil durum tokenı");
        }

        // Sadece kritik secret'ları döndür
        var criticalSecrets = new Dictionary<string, string>
        {
            ["DatabaseSettings:ConnectionString"] = _secretsManager.GetSecret("DatabaseSettings:ConnectionString"),
            ["TokenOptions:SecurityKey"] = _secretsManager.GetSecret("TokenOptions:SecurityKey")
        };

        _auditService.LogAccess("ALL", "EMERGENCY_ACCESS_GRANTED", "EmergencyUser", requesterIp);

        return criticalSecrets;
    }

    public void RotateEmergencyToken()
    {
        var newToken = GenerateSecureToken();
        var validUntil = DateTime.UtcNow.AddHours(4); // 4 saat geçerli

        // Yeni token'ı güvenli bir şekilde dağıt
        DistributeEmergencyToken(newToken, validUntil);

        // Token'ı konfigürasyona kaydet (geçici olarak)
        _configuration["Emergency:AccessToken"] = newToken;
        _configuration["Emergency:ValidUntil"] = validUntil.ToString("O");

        _logger.LogInformation("Acil durum tokenı başarıyla döndürüldü");
    }

    private string GenerateSecureToken()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }

    private void DistributeEmergencyToken(string newToken, DateTime validUntil)
    {
        var recipients = _configuration["Emergency:NotificationRecipients"]?.Split(',');
        if (recipients == null || recipients.Length == 0)
        {
            _logger.LogWarning("Acil durum bildirim alıcıları tanımlanmamış");
            return;
        }

        foreach (var recipient in recipients)
            try
            {
                var trimmedRecipient = recipient.Trim();
                if (trimmedRecipient.Contains("@"))
                    SendEmergencyEmail(trimmedRecipient, newToken, validUntil);
                else
                    SendEmergencySMS(trimmedRecipient, newToken, validUntil);

                _logger.LogInformation("Acil durum tokenı {Recipient} adresine gönderildi", trimmedRecipient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Acil durum tokenı {Recipient} adresine gönderilemedi", recipient);
            }
    }

    private void SendEmergencyEmail(string email, string token, DateTime validUntil)
    {
        var smtpServer = _configuration["Emergency:SmtpServer"] ?? "smtp.gmail.com";
        var smtpPort = int.Parse(_configuration["Emergency:SmtpPort"] ?? "587");
        var smtpUser = _configuration["Emergency:SmtpUser"];
        var smtpPass = _configuration["Emergency:SmtpPass"];

        using (var client = new SmtpClient(smtpServer, smtpPort))
        {
            client.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPass);
            client.EnableSsl = true;

            var message = new MailMessage
            {
                From = new MailAddress(smtpUser),
                Subject = "ACİL DURUM ERİŞİM TOKENI",
                Body = $@"
ACİL DURUM ERİŞİM BİLGİLERİ

Token: {token}
Geçerlilik Süresi: {validUntil:dd.MM.yyyy HH:mm:ss} UTC

Bu token'ı sadece acil durumlarda kullanın. Token'ın güvenliğini sağlayın ve kullanımdan sonra imha edin.

Güvenlik Uyarısı:
- Bu token 4 saat boyunca geçerlidir
- Token'ı kimseyle paylaşmayın
- İşiniz bittikten sonra token'ı tekrar döndürün
",
                IsBodyHtml = false,
                Priority = MailPriority.High
            };

            message.To.Add(email);

            client.Send(message);
        }
    }

    private void SendEmergencySMS(string phoneNumber, string token, DateTime validUntil)
    {
        var apiUrl = _configuration["Emergency:SmsApiUrl"];
        var apiKey = _configuration["Emergency:SmsApiKey"];

        if (string.IsNullOrEmpty(apiUrl) || string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("SMS API konfigürasyonu eksik");
            return;
        }

        using (var client = new HttpClient())
        {
            var message = $"ACIL DURUM TOKENI: {token} - Gecerli until: {validUntil:HH:mm} UTC";

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("apiKey", apiKey),
                new KeyValuePair<string, string>("to", phoneNumber),
                new KeyValuePair<string, string>("message", message),
                new KeyValuePair<string, string>("from", "EMERGENCY")
            });

            var response = client.PostAsync(apiUrl, content).Result;
            if (!response.IsSuccessStatusCode)
                throw new Exception($"SMS gönderilemedi: {response.StatusCode}");
        }
    }

    public async Task<bool> ValidateAndExtendTokenAsync(string token, string requesterInfo)
    {
        if (!ValidateEmergencyToken(token))
            return false;

        // Token geçerliyse süresini uzat
        RotateEmergencyToken();

        _auditService.LogAccess("EMERGENCY_TOKEN", "TOKEN_EXTENDED", requesterInfo, "SYSTEM");
        return true;
    }
}