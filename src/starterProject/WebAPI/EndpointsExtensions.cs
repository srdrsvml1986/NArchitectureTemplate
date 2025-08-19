using Application.Services;
using Microsoft.AspNetCore.Mvc;
using WebAPI;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapEmergencyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Acil durum secret erişim endpoint'i
        endpoints.MapPost("/emergency/secrets", async context =>
        {
            var emergencyService = context.RequestServices.GetService<EmergencyAccessService>();
            var request = await context.Request.ReadFromJsonAsync<EmergencyAccessRequest>();

            if (request == null || string.IsNullOrEmpty(request.Token))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Token gereklidir");
                return;
            }

            try
            {
                var secrets = emergencyService.GetSecretsForEmergency(
                    request.Token,
                    context.Connection.RemoteIpAddress?.ToString()
                );

                await context.Response.WriteAsJsonAsync(new
                {
                    Status = "EmergencyAccessGranted",
                    Secrets = secrets.Keys // Sadece key'leri döndür, değerleri değil
                });
            }
            catch (UnauthorizedAccessException)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Geçersiz token");
            }
        });

        // Acil durum token yenileme endpoint'i
        endpoints.MapPost("/emergency/rotate-token", async context =>
        {
            var emergencyService = context.RequestServices.GetService<EmergencyAccessService>();
            var request = await context.Request.ReadFromJsonAsync<EmergencyTokenRequest>();

            if (request == null || string.IsNullOrEmpty(request.AdminPassword))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Admin şifresi gereklidir");
                return;
            }

            // Basit bir şifre kontrolü - production'da daha güvenli bir yöntem kullanın
            if (request.AdminPassword != "PredefinedEmergencyPassword123!")
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Geçersiz admin şifresi");
                return;
            }

            emergencyService.RotateEmergencyToken();
            await context.Response.WriteAsync("Acil durum tokenı yenilendi");
        });

        // Acil durum bildirim test endpoint'i
        endpoints.MapPost("/emergency/test-notification", async context =>
        {
            var notificationService = context.RequestServices.GetService<EmergencyNotificationService>();
            var request = await context.Request.ReadFromJsonAsync<EmergencyNotificationRequest>();

            if (request == null || string.IsNullOrEmpty(request.Message))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Mesaj gereklidir");
                return;
            }

            await notificationService.SendEmergencyAlertAsync(
                request.Severity,
                request.Message
            );

            await context.Response.WriteAsync("Acil durum bildirimi gönderildi");
        });

        // Acil durum durum sorgulama endpoint'i
        endpoints.MapGet("/emergency/status", async context =>
        {
            var emergencyService = context.RequestServices.GetService<EmergencyAccessService>();
            var disasterRecoveryService = context.RequestServices.GetService<DisasterRecoveryService>();
            var auditService = context.RequestServices.GetService<AuditService>();

            var status = new
            {
                EmergencyAccessEnabled = true,
                LastBackupTime = DateTime.UtcNow.AddHours(-1),
                BackupCount = disasterRecoveryService?.GetAvailableBackups()?.Length ?? 0,
                RecentEmergencyAccess = auditService?.GetRecentAuditLogs(10)?.Where(log => log.Contains("EMERGENCY"))?.ToArray() ?? Array.Empty<string>()
            };

            await context.Response.WriteAsJsonAsync(status);
        });

        // Yedekleri listeleme endpoint'i
        endpoints.MapGet("/emergency/backups", async context =>
        {
            var disasterRecoveryService = context.RequestServices.GetService<DisasterRecoveryService>();
            var backups = disasterRecoveryService?.GetAvailableBackups() ?? Array.Empty<string>();

            await context.Response.WriteAsJsonAsync(new
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
        }).RequireAuthorization("AdminPolicy");

        // Health check endpoint'ine ekle
        endpoints.MapGet("/health/detailed", async (
            [FromServices] ILocalSecretsManager secretsManager,
            [FromServices] EmergencyNotificationService notificationService) =>
        {
            try
            {
                // Secrets manager sağlık kontrolü
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