using Application.Services;
using Microsoft.AspNetCore.Diagnostics;
using System.Security;

namespace WebAPI.Middleware;
public static class CustomExceptionHandler
{
    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(exceptionApp =>
        {
            exceptionApp.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;

                var notificationService = context.RequestServices.GetService<EmergencyNotificationService>();

                if (exception is SecurityException || exception is UnauthorizedAccessException)
                    await notificationService.SendEmergencyAlertAsync(
                        "SECURITY_BREACH",
                        $"Güvenlik ihlali tespit edildi: {exception.Message}"
                    );

                // Diğer hata türleri için...
            });
        });
    }
}


