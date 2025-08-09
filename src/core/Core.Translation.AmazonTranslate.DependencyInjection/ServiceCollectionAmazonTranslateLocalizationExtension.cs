using Microsoft.Extensions.DependencyInjection;
using NArchitectureTemplate.Core.Translation.Abstraction;
using NArchitectureTemplate.Core.Translation.AmazonTranslate;

namespace NArchitectureTemplate.Core.Translation.AmazonTranslate.DependencyInjection;

public static class ServiceCollectionAmazonTranslateLocalizationExtension
{
    public static IServiceCollection AddAmazonTranslation(
        this IServiceCollection services,
        AmazonTranslateConfiguration configuration
    )
    {
        services.AddTransient<ITranslationService, AmazonTranslateLocalizationService>(
            _ => new AmazonTranslateLocalizationService(configuration)
        );
        return services;
    }
}
