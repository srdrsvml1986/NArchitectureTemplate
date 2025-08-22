using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Services.EmergencyAndSecretServices;
public class BackupService : BackgroundService
{
    private readonly ILogger<BackupService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _backupInterval = TimeSpan.FromHours(6); // 6 saatte bir

    public BackupService(ILogger<BackupService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Backup Service başlatıldı. Yedekleme aralığı: {BackupInterval}", _backupInterval);

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var disasterRecoveryService = scope.ServiceProvider.GetRequiredService<DisasterRecoveryService>();
                    var secretsFilePath = "secrets.dat"; // Varsayılan dosya yolu

                    await disasterRecoveryService.CreateBackupAsync(secretsFilePath);
                    disasterRecoveryService.CleanOldBackups(30); // 30 yedekten eski olanları temizle
                }

                await Task.Delay(_backupInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Backup Service'de hata oluştu");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // 5 dakika bekle ve tekrar dene
            }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Backup Service durduruluyor...");
        await base.StopAsync(cancellationToken);
    }
}