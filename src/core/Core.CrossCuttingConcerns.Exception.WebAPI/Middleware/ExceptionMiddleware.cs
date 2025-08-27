using Microsoft.AspNetCore.Http;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.WebApi.Handlers;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction;
using NArchitectureTemplate.Core.Notification.Services;
using NArchitectureTemplate.Core.Security.Entities;
using System.Net.Mime;
using System.Security;
using System.Text.Json;
using WebAPI;

namespace NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.WebApi.Middleware;

public class ExceptionMiddleware
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly HttpExceptionHandler _httpExceptionHandler;
    private readonly ILogger _loggerService;
    private readonly INotificationService _notificationService;
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(
        RequestDelegate next,
        IHttpContextAccessor contextAccessor,
        ILogger loggerService,
        INotificationService notificationService)
    {
        _next = next;
        _contextAccessor = contextAccessor;
        _loggerService = loggerService;
        _notificationService = notificationService;
        _httpExceptionHandler = new HttpExceptionHandler();
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (System.Exception exception)
        {
            await LogException(context, exception);
            await HandleExceptionAsync(context.Response, exception);

            // Şüpheli durumlarda bildirim gönder
            if (IsSuspiciousException(exception))
            {
                await NotifySuspiciousActivity(exception, context);
            }
        }
    }

    private bool IsSuspiciousException(System.Exception exception)
    {
        // Şüpheli sayılacak exception türlerini burada belirleyin
        return exception is UnauthorizedAccessException
            || exception is SecurityException
            || exception.Message.Contains("suspicious", StringComparison.OrdinalIgnoreCase);
    }

    private async Task NotifySuspiciousActivity(System.Exception exception, HttpContext context)
    {
        try
        {
            var session = new UserSession<Guid, Guid>(
                id: Guid.NewGuid(),
                userId: GetUserIdFromContext(context),
                ipAddress: context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                userAgent: context.Request.Headers.UserAgent.ToString()
            )
            {
                IsSuspicious = true,
                LocationInfo = "Detected from exception middleware",
                LoginTime = DateTime.UtcNow
            };

            await _notificationService.NotifySuspiciousSessionAsync(session);
        }
        catch (System.Exception ex)
        {
            _loggerService.Error(ex, "Bildirim gönderilirken hata oluştu");
        }
    }

    private Guid GetUserIdFromContext(HttpContext context)
    {
        // Kullanıcı ID'sini context'ten alın
        // Bu kısmı kendi authentication yapınıza göre doldurun
        var userIdClaim = context.User.FindFirst("userId");
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return Guid.Empty;
    }

    protected virtual Task HandleExceptionAsync(HttpResponse response, dynamic exception)
    {
        response.ContentType = MediaTypeNames.Application.Json;
        _httpExceptionHandler.Response = response;

        return _httpExceptionHandler.HandleException(exception);
    }

    protected virtual Task LogException(HttpContext context, System.Exception exception)
    {
        List<LogParameter> logParameters = [new LogParameter { Type = context.GetType().Name, Value = exception.ToString() }];

        LogDetail logDetail =
            new()
            {
                MethodName = _next.Method.Name,
                Parameters = logParameters,
                User = _contextAccessor.HttpContext?.User.Identity?.Name ?? "?"
            };

        try
        {
            string logDetailJson = JsonSerializer.Serialize(logDetail);
            _loggerService.Error(exception, $"ExceptionMiddleware: {logDetailJson}");
        }
        catch (System.Exception exx)
        {
        }
        return Task.CompletedTask;
    }
}