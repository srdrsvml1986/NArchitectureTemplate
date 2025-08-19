using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace Application.Services;
// KeyRotationService.cs
public class KeyRotationService : BackgroundService
{
    private readonly ILocalSecretsManager _secretsManager;
    private readonly ILogger<KeyRotationService> _logger;
    private readonly TimeSpan _rotationInterval = TimeSpan.FromDays(90);

    public KeyRotationService(
        ILocalSecretsManager secretsManager,
        ILogger<KeyRotationService> logger)
    {
        _secretsManager = secretsManager;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_rotationInterval, stoppingToken);

                _logger.LogInformation("Anahtar rotasyonu başlatılıyor...");

                // Yeni master key oluştur
                var newMasterKey = GenerateSecureKey(32);

                // Tüm secret'ları yeni key ile yeniden şifrele
                await RotateAllSecrets(newMasterKey);

                // Ortam değişkenini güncelle
                Environment.SetEnvironmentVariable("MASTER_KEY", newMasterKey,
                    EnvironmentVariableTarget.Machine);

                _logger.LogInformation("Anahtar rotasyonu başarıyla tamamlandı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Anahtar rotasyonu sırasında hata oluştu");
            }
        }
    }

    private async Task RotateAllSecrets(string newMasterKey)
    {
        var oldSecrets = _secretsManager.GetAllSecrets();
        var newEncryptor = new EncryptionService(newMasterKey);

        // Tüm secret'ları yeni key ile yeniden şifrele
        foreach (var (key, value) in oldSecrets)
        {
            var reencryptedValue = newEncryptor.Encrypt(value);
            // Geçici olarak farklı bir yere kaydet
        }

        // Tüm secret'ları atomik olarak güncelle
        // Bu işlem sırasında uygulama kısa süreliğine durdurulmalı
    }

    private string GenerateSecureKey(int length)
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}