using Application.Services.EmergencyAndSecretServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Extensions;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapEmergencyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // 1. Acil durum secret erişim endpoint'i
        endpoints.MapPost("/emergency/secrets", async (
            [FromBody] EmergencyAccessRequest request,
            [FromServices] EmergencyAccessService emergencyService,
            HttpContext context) =>
        {
            if (request == null || string.IsNullOrEmpty(request.Token))
            {
                return Results.BadRequest("Token gereklidir");
            }

            try
            {
                var secrets = emergencyService.GetSecretsForEmergency(
                    request.Token,
                    context.Connection.RemoteIpAddress?.ToString()
                );

                return Results.Ok(new
                {
                    Status = "EmergencyAccessGranted",
                    Secrets = secrets.Keys // Sadece key'leri döndür, değerleri değil
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Forbid();
            }
        });

        // 2. Acil durum token yenileme endpoint'i
        endpoints.MapPost("/emergency/rotate-token", (
            [FromServices] EmergencyAccessService emergencyService) =>
        {


            emergencyService.RotateEmergencyToken();
            return Results.Ok("Acil durum tokenı yenilendi");
        });

        // 3. Acil durum bildirim test endpoint'i
        endpoints.MapPost("/emergency/test-notification", async (
            [FromBody] EmergencyNotificationRequest request,
            [FromServices] EmergencyNotificationService notificationService) =>
        {
            if (request == null || string.IsNullOrEmpty(request.Message))
            {
                return Results.BadRequest("Mesaj gereklidir");
            }

            await notificationService.SendEmergencyAlertAsync(
                request.Severity,
                request.Message
            );

            return Results.Ok("Acil durum bildirimi gönderildi");
        });

        // 4. Acil durum durum sorgulama endpoint'i
        endpoints.MapGet("/emergency/status", (
            [FromServices] EmergencyAccessService emergencyService,
            [FromServices] DisasterRecoveryService disasterRecoveryService,
            [FromServices] AuditService auditService) =>
        {
            var status = new
            {
                EmergencyAccessEnabled = true,
                LastBackupTime = DateTime.UtcNow.AddHours(-1),
                BackupCount = disasterRecoveryService.GetAvailableBackups().Length,
                RecentEmergencyAccess = auditService.GetRecentAuditLogs(10)
                    .Where(log => log.Contains("EMERGENCY"))
                    .ToArray()
            };

            return Results.Ok(status);
        });

        // 5. Yedekleri listeleme endpoint'i
        endpoints.MapGet("/emergency/backups", (
            [FromServices] DisasterRecoveryService disasterRecoveryService) =>
        {
            var backups = disasterRecoveryService.GetAvailableBackups();

            return Results.Ok(new
            {
                Count = backups.Length,
                Backups = backups
            });
        });

        return endpoints;
    }
    public static IEndpointRouteBuilder MapSecretManagerEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // 1. Secret Ekleme/Güncelleme
        endpoints.MapPost("/secrets", (
            [FromBody] SecretRequest request,
            [FromServices] ILocalSecretsManager sm,
            [FromServices] AuditService audit,
            HttpContext context) =>
        {
            sm.SetSecret(request.Key, request.Value);
            // Denetim kaydı ekle
            audit.LogAccess(
                request.Key,
                "SET_SECRET",
                context.User.Identity?.Name ?? "Anonymous",
                context.Connection.RemoteIpAddress?.ToString() ?? ""
            );
            return Results.Ok(new
            {
                Status = "Success",
                Message = $"Secret '{request.Key}' saved"
            });
        });

        // 2. Secret Getirme
        endpoints.MapGet("/secrets/{key}", (
            string key,
            [FromServices] ILocalSecretsManager sm) =>
        {
            var value = sm.GetSecret(key);
            return value != null
                ? Results.Ok(new { Key = key, Value = value })
                : Results.NotFound($"Secret '{key}' not found");
        });

        // 3. Secret Güncelleme
        endpoints.MapPut("/secrets/{key}", (
            string key,
            [FromBody] UpdateSecretRequest request,
            [FromServices] ILocalSecretsManager sm) =>
        {
            try
            {
                sm.UpdateSecret(key, request.NewValue);
                return Results.Ok(new
                {
                    Status = "Success",
                    Message = $"Secret '{key}' updated"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(ex.Message);
            }
        });

        // 4. Secret Silme
        endpoints.MapDelete("/secrets/{key}", (
            string key,
            [FromServices] ILocalSecretsManager sm) =>
        {
            try
            {
                sm.DeleteSecret(key);
                return Results.Ok(new
                {
                    Status = "Success",
                    Message = $"Secret '{key}' deleted"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(ex.Message);
            }
        });

        // 5. Tüm Secret'ları Listeleme
        endpoints.MapGet("/secrets", (
            [FromServices] ILocalSecretsManager sm) =>
        {
            var allSecrets = sm.GetAllSecrets();
            return Results.Ok(allSecrets);
        });

        // 6. Secret Varlık Kontrolü
        endpoints.MapGet("/secrets/{key}/exists", (
            string key,
            [FromServices] ILocalSecretsManager sm) =>
        {
            return Results.Ok(new
            {
                Key = key,
                Exists = sm.SecretExists(key)
            });
        });

        // Yönetici dashboard endpoint'leri
        endpoints.MapGet("/admin/emergency-status", (
            [FromServices] EmergencyAccessService emergencyService,
            [FromServices] AuditService auditService) =>
        {
            var recentAccess = auditService.GetRecentAuditLogs(50)
                .Where(log => log.Contains("EMERGENCY"))
                .ToList();

            return Results.Ok(new
            {
                EmergencyAccessEnabled = true,
                RecentAccessAttempts = recentAccess
            });
        });

        // Health check endpoint'ine ekle
        endpoints.MapGet("/health/detailed", async (
            [FromServices] ILocalSecretsManager secretsManager,
            [FromServices] EmergencyNotificationService notificationService) =>
        {
            try
            {
                // Secrets manager sağlık kontrolü
                secretsManager.SetSecret("HealthCheck_Test", "HealthCheck_Test");
                var testResult = secretsManager.GetSecret("HealthCheck_Test");
                if (testResult == null)
                {
                    await notificationService.SendEmergencyAlertAsync(
                        "SEVERE",
                        "Secrets manager erişilemez durumda!"
                    );
                    return Results.Problem("Secrets manager erişilemiyor");
                }

                return Results.Ok("Sistem sağlıklı");
            }
            catch (Exception ex)
            {
                await notificationService.SendEmergencyAlertAsync(
                    "CRITICAL",
                    $"Sistem sağlık kontrolü başarısız: {ex.Message}"
                );
                return Results.Problem("Sistem hatası");
            }
        });

        endpoints.MapGet("/health/secrets", ([FromServices] ILocalSecretsManager sm) =>
        {
            try
            {
                var testKey = "HealthCheck_TestKey";
                var testValue = Guid.NewGuid().ToString();

                // Yazma testi
                sm.SetSecret(testKey, testValue);

                // Okuma testi
                var readValue = sm.GetSecret(testKey);

                // Temizle
                sm.DeleteSecret(testKey);

                return readValue == testValue
                    ? Results.Ok("Secrets manager sağlıklı")
                    : Results.Problem("Secrets manager hatası");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Secrets manager erişim hatası: {ex.Message}");
            }
        });

        endpoints.MapGet("/health/backup", ([FromServices] DisasterRecoveryService drs) =>
        {
            try
            {
                var backups = drs.GetAvailableBackups();
                return backups.Any()
                    ? Results.Ok($"{backups.Length} backup mevcut")
                    : Results.Problem("Backup bulunamadı");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Backup kontrol hatası: {ex.Message}");
            }
        });

        return endpoints;
    }
}