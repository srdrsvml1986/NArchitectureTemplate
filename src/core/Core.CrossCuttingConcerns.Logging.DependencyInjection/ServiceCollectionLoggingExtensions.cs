using Microsoft.Extensions.DependencyInjection;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction;

namespace NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.DependencyInjection;

public static class ServiceCollectionLoggingExtensions
{
    public static IServiceCollection AddLogging(this IServiceCollection services, ILogger logger)
    {
        services.AddSingleton(logger);

        return services;
    }
}
