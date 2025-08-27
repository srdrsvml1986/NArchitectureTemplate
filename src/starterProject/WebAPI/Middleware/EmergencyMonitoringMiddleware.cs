// EmergencyMonitoringMiddleware.cs
using Application.Services.EmergencyAndSecretServices;
using Microsoft.Extensions.DependencyInjection;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction;
using NArchitectureTemplate.Core.Notification.Services;
using System.Text;
using ILogger = NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction.ILogger;

namespace WebAPI.Middleware;

public class EmergencyMonitoringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public EmergencyMonitoringMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Request body'yi önceden okuyup yedekleyelim
        string originalBody = string.Empty;
        Stream originalBodyStream = null;

        try
        {
            // Sadece acil durum header'ı varsa işlem yap
            if (context.Request.Headers.ContainsKey("X-Emergency-Access"))
            {
                // Request body'yi yedekle ve tekrar okunabilir hale getir
                context.Request.EnableBuffering();
                originalBodyStream = context.Request.Body;
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                originalBody = await reader.ReadToEndAsync();
                context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(originalBody));
                context.Request.Body.Seek(0, SeekOrigin.Begin);

                var emergencyService = context.RequestServices.GetService<EmergencyAccessService>();
                var auditService = context.RequestServices.GetService<AuditService>();

                var emergencyToken = context.Request.Headers["X-Emergency-Access"].ToString();

                if (emergencyService != null && emergencyService.ValidateEmergencyToken(emergencyToken))
                {
                    auditService?.LogAccess(
                        "EMERGENCY_ACCESS",
                        "EMERGENCY_REQUEST",
                        context.User.Identity?.Name ?? "Unknown",
                        context.Connection.RemoteIpAddress?.ToString()
                    );
                }
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "EmergencyMonitoringMiddleware hatası");

            var notificationService = context.RequestServices.GetService<EmergencyNotificationService>();
            if (notificationService != null)
            {
                //await notificationService.NotifySecurityBreachAsync(
                //    "MIDDLEWARE_ERROR",
                //    $"EmergencyMonitoringMiddleware hatası: {ex.Message}"
                //);
            }

            // Hata durumunda bile request pipeline'ı devam ettir
            // Ancak önce body'yi orijinal haline getir
            if (originalBodyStream != null)
            {
                context.Request.Body = originalBodyStream;
            }

            await _next(context);
        }
        finally
        {
            // Body'yi orijinal haline getir
            if (originalBodyStream != null)
            {
                context.Request.Body = originalBodyStream;
            }
        }
    }
}