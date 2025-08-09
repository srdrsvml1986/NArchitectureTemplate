using Microsoft.AspNetCore.Builder;

namespace NArchitectureTemplate.Core.Localization.WebApi;

public static class ApplicationBuilderLocalizationMiddlewareExtensions
{
    public static IApplicationBuilder UseResponseLocalization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LocalizationMiddleware>();
    }
}
