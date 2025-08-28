using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction;
using NArchitectureTemplate.Core.Notification.Services;

namespace Application.Services.EmergencyAndSecretServices;
public class HealthCheckService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _healthCheckInterval = TimeSpan.FromMinutes(5); // 5 dakikada bir

    public HealthCheckService(ILogger logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Information("HealthCheck Service başlatıldı. Kontrol aralığı: {HealthCheckInterval}", _healthCheckInterval);

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var secretsManager = scope.ServiceProvider.GetRequiredService<ILocalSecretsManager>();
                    var notificationService = scope.ServiceProvider.GetRequiredService<EmergencyNotificationService>();

                    // Secrets manager sağlık kontrolü
                    var testKey = "HealthCheck_TestKey";
                    var testValue = Guid.NewGuid().ToString();

                    secretsManager.SetSecret(testKey, testValue);
                    var retrievedValue = secretsManager.GetSecret(testKey);
                    secretsManager.DeleteSecret(testKey);

                    if (retrievedValue != testValue)
                        await notificationService.NotifySecurityBreachAsync(
                            "SEVERE",
                            "Secrets manager sağlık kontrolü başarısız!"
                        );
                    else
                        _logger.Information("Secrets manager sağlık kontrolü başarılı");
                }

                await Task.Delay(_healthCheckInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "HealthCheck Service'de hata oluştu");
                await Task.Delay(TimeSpan.FromMinutes(100), stoppingToken); // 100 dakika bekle ve tekrar dene
            }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Information("HealthCheck Service durduruluyor...");
        await base.StopAsync(cancellationToken);
    }
}
