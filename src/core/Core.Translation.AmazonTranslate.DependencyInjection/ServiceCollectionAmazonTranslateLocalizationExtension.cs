﻿using Microsoft.Extensions.DependencyInjection;
using NArchitecture.Core.Translation.Abstraction;
using NArchitecture.Core.Translation.AmazonTranslate;

namespace NArchitecture.Core.Translation.AmazonTranslate.DependencyInjection;

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
