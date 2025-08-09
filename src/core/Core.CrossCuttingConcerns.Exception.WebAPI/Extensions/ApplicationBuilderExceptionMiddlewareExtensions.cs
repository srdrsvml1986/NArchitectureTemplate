using Microsoft.AspNetCore.Builder;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.WebApi.Middleware;

namespace NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.WebApi.Extensions;

public static class ApplicationBuilderExceptionMiddlewareExtensions
{
    public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}
