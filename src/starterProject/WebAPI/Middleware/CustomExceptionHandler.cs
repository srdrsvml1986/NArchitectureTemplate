using Application.Services.EmergencyAndSecretServices;
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
                var logger = context.Features.Get<NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction.ILogger>();
                var exception = exceptionHandlerPathFeature?.Error;

                var notificationService = context.RequestServices.GetService<EmergencyNotificationService>();

                if ((exception is SecurityException || exception is UnauthorizedAccessException)&& exception is not null)
                    logger?.Error(exception,"Güvenlik ihlali: " +
                        context.User.Identity?.Name + ", " +
                        context.Connection.RemoteIpAddress?.ToString() + ": " + exception.Message);

                if (notificationService is not null)
                await notificationService.NotifySecurityBreachAsync(
                        "SECURITY_BREACH",
                        $"Güvenlik ihlali: {exception?.Message} - User: {context.User.Identity?.Name} - IP: {context.Connection.RemoteIpAddress}"
                    );
            });
        });
    }
}


