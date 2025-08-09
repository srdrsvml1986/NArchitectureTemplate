using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NArchitectureTemplate.Core.Security.OAuth.Middleware;
using NArchitectureTemplate.Core.Security.OAuth.Services;

namespace NArchitectureTemplate.Core.Security.OAuth.Extensions;

public static class OAuthMiddlewareExtensions
{
    public static IServiceCollection AddOAuthSecurity(this IServiceCollection services)
    {
        // Antiforgery servisini ekle
        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-XSRF-TOKEN";
            options.Cookie.Name = "XSRF-TOKEN";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

        // OAuth servislerini ekle
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IFacebookAuthService, FacebookAuthService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }

    public static IApplicationBuilder UseOAuthSecurity(this IApplicationBuilder app)
    {
        return app.UseMiddleware<OAuthRateLimitMiddleware>();
    }
}
