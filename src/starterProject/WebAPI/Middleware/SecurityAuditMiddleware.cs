using Application.Services.EmergencyAndSecretServices;

namespace WebAPI.Middleware;
public class SecurityAuditMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityAuditMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var auditService = context.RequestServices.GetService<AuditService>();

        try
        {
            // Tüm istekleri denetle
            auditService.LogAccess(
                "REQUEST",
                context.Request.Method,
                context.User.Identity?.Name ?? "Anonymous",
                context.Connection.RemoteIpAddress?.ToString(),
                $"{context.Request.Path}{context.Request.QueryString}"
            );

            await _next(context);

            // Yanıt durum kodunu da denetle
            auditService.LogAccess(
                "RESPONSE",
                $"STATUS_{context.Response.StatusCode}",
                context.User.Identity?.Name ?? "Anonymous",
                context.Connection.RemoteIpAddress?.ToString(),
                $"Path: {context.Request.Path}, Status: {context.Response.StatusCode}"
            );
        }
        catch (Exception ex)
        {
            var notificationService = context.RequestServices.GetService<EmergencyNotificationService>();
            await notificationService.NotifySecurityBreachAsync(
                "AUDIT_ERROR",
                $"SecurityAuditMiddleware hatası: {ex.Message}"
            );
        }
    }
}
