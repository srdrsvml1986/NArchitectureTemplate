using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace NArchitecture.Core.Security.OAuth.Middleware;

public class OAuthRateLimitMiddleware
{
    private readonly RateLimiter _loginLimiter;
    private readonly IAntiforgery _antiforgery;

    public OAuthRateLimitMiddleware(IAntiforgery antiforgery)
    {
        _antiforgery = antiforgery;
        _loginLimiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(5),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.StartsWithSegments("/auth"))
        {
            // CSRF koruması
            if (!await ValidateAntiForgeryToken(context))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid request");
                return;
            }

            // Rate limiting
            if (!await CheckRateLimit(context))
            {
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Too many requests");
                return;
            }
        }

        await next(context);
    }

    private async Task<bool> CheckRateLimit(HttpContext context)
    {
        using var lease = await _loginLimiter.AcquireAsync(1);
        return lease.IsAcquired;
    }

    private async Task<bool> ValidateAntiForgeryToken(HttpContext context)
    {
        try
        {
            // POST, PUT, DELETE gibi güvenlik gerektiren istekler için token doğrulama
            if (!HttpMethods.IsGet(context.Request.Method))
            {
                await _antiforgery.ValidateRequestAsync(context);
            }

            return true;
        }
        catch (AntiforgeryValidationException)
        {
            return false;
        }
    }
}

