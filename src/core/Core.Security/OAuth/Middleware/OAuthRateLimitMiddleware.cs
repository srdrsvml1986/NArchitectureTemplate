using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace NArchitectureTemplate.Core.Security.OAuth.Middleware;

public class OAuthRateLimitMiddleware
{
    private readonly RateLimiter _loginLimiter;
    private readonly IAntiforgery _antiforgery;
    private readonly RequestDelegate _next;

    // Constructor'ı RateLimiter'ı da parametre olarak alacak şekilde değiştir
    public OAuthRateLimitMiddleware(IAntiforgery antiforgery, RateLimiter loginLimiter, RequestDelegate next)
    {
        _antiforgery = antiforgery;
        _loginLimiter = loginLimiter; // DI'dan gelen RateLimiter'ı kullan
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/auth"))
        {
            // CSRF koruması
            if (!await ValidateAntiForgeryToken(context))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Geçersiz istek");
                return;
            }

            // Rate limiting
            if (!await CheckRateLimit(context))
            {
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Çok fazla istekte bulunuldu. Lütfen 5dk sonra tekrar deneyiniz");
                return;
            }
        }

        await _next(context);
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

