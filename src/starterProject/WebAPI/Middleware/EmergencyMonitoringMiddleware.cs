using Application.Services;

namespace WebAPI.Middleware;
public class EmergencyMonitoringMiddleware
{
    private readonly RequestDelegate _next;

    public EmergencyMonitoringMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Acil durum izleme burada yapılır
        var emergencyService = context.RequestServices.GetService<EmergencyAccessService>();
        var auditService = context.RequestServices.GetService<AuditService>();

        try
        {
            // Özel acil durum header'ını kontrol et
            if (context.Request.Headers.ContainsKey("X-Emergency-Access"))
            {
                var emergencyToken = context.Request.Headers["X-Emergency-Access"].ToString();

                if (emergencyService.ValidateEmergencyToken(emergencyToken))
                    auditService.LogAccess(
                        "EMERGENCY_ACCESS",
                        "EMERGENCY_REQUEST",
                        context.User.Identity?.Name ?? "Unknown",
                        context.Connection.RemoteIpAddress?.ToString()
                    );
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            var notificationService = context.RequestServices.GetService<EmergencyNotificationService>();
            await notificationService.SendEmergencyAlertAsync(
                "MIDDLEWARE_ERROR",
                $"EmergencyMonitoringMiddleware hatası: {ex.Message}"
            );

            throw;
        }
    }
}