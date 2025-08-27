using Application.Services;
using Application.Services.AuthService;
using Application.Services.EmergencyAndSecretServices;
using Application.Services.ExceptionLogs;
using Application.Services.GroupOperationClaims;
using Application.Services.GroupRoles;
using Application.Services.Groups;
using Application.Services.LoggerService;
using Application.Services.Logs;
using Application.Services.NotificationServices;
using Application.Services.PasswordResetTokens;
using Application.Services.RoleOperationClaims;
using Application.Services.Roles;
using Application.Services.UserGroups;
using Application.Services.UserRoles;
using Application.Services.UserSessions;
using Application.Services.UsersService;
using FluentValidation;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Pipelines.Caching;
using NArchitectureTemplate.Core.Application.Pipelines.Logging;
using NArchitectureTemplate.Core.Application.Pipelines.Transaction;
using NArchitectureTemplate.Core.Application.Pipelines.Validation;
using NArchitectureTemplate.Core.Application.Rules;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Configurations;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Serilog.File;
using NArchitectureTemplate.Core.ElasticSearch;
using NArchitectureTemplate.Core.ElasticSearch.Models;
using NArchitectureTemplate.Core.Localization.Resource.Yaml.DependencyInjection;
using NArchitectureTemplate.Core.Mailing;
using NArchitectureTemplate.Core.Mailing.MailKit;
using NArchitectureTemplate.Core.Security.DependencyInjection;
using NArchitectureTemplate.Core.Security.JWT;
using System.Reflection;
using Application.Services.DeviceTokens;
using Application.Services.UserNotificationSettings;
using NArchitectureTemplate.Core.Notification.Services;

namespace Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        MailSettings mailSettings,
        FileLogConfiguration fileLogConfiguration,
        ElasticSearchConfig elasticSearchConfig,
        TokenOptions tokenOptions,
        LoggingConfig loggingConfig
    )
    {
        services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            configuration.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            configuration.AddOpenBehavior(typeof(AdvancedAuthorizationBehavior<,>));
            configuration.AddOpenBehavior(typeof(CachingBehavior<,>));
            configuration.AddOpenBehavior(typeof(CacheRemovingBehavior<,>));
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(TransactionScopeBehavior<,>));
        });

        services.AddSubClassesOfType(Assembly.GetExecutingAssembly(), typeof(BaseBusinessRules));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddSingleton<IElasticSearch, ElasticSearchService>(_ => new ElasticSearchService(elasticSearchConfig));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();

        services.AddYamlResourceLocalization();

        services.AddSecurityServices<Guid, int, int, int, Guid>(tokenOptions);

        services.AddScoped<ICurrentUserAuthorizationService, CurrentUserAuthorizationService>();

        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IGroupOperationClaimService, GroupOperationClaimService>();
        services.AddScoped<IUserGroupService, UserGroupService>();
        services.AddScoped<IPasswordResetTokenService, PasswordResetTokenService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IRoleOperationClaimService, RoleOperationClaimService>();
        services.AddScoped<IGroupRoleService, GroupRoleService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IEmailSender, NoOpEmailSender>();
        services.AddScoped<IUserSessionService, UserSessionService>();
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<IExceptionLogService, ExceptionLogService>();
        services.AddSingleton<AuditService>();
        services.AddSingleton<DisasterRecoveryService>();
        services.AddSingleton<EmergencyAccessService>();
        services.AddSingleton<EmergencyNotificationService>();

        // Background servisler
        services.AddHostedService<BackupService>();
        services.AddHostedService<HealthCheckService>();


        services.AddSingleton<IMailService, MailKitMailService>(_ => new MailKitMailService(mailSettings));


        // Logger servislerini kaydet
        services.AddSingleton<DatabaseLogger>();
        services.AddSingleton<SerilogFileLogger>(_ =>
        {
            try
            {
                return new SerilogFileLogger(fileLogConfiguration);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SerilogFileLogger olu�turulurken hata: {ex.Message}");
                // Fallback: Basit bir console logger d�nd�r
                return new SerilogFileLogger(new FileLogConfiguration("/logs/"));
            }
        });

        services.AddSingleton<ILogger>(sp =>
        {
            var loggers = new List<ILogger>();

            // SerilogFileLogger'dene
            try
            {
                var fileLogger = sp.GetRequiredService<SerilogFileLogger>();
                loggers.Add(fileLogger);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SerilogFileLogger al�namad�: {ex.Message}");
            }

            // DatabaseLogger' ekle
            try
            {
                var dbLogger = sp.GetRequiredService<DatabaseLogger>();
                loggers.Add(dbLogger);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DatabaseLogger al�namad�: {ex.Message}");
            }

            // Hi logger yoksa fallback
            if (!loggers.Any())
            {
                loggers.Add(new ConsoleFallbackLogger());
            }

            // Hedef se�imine g�re filtrele
            IEnumerable<ILogger> filteredLoggers = loggingConfig.Target?.ToLowerInvariant() switch
            {
                "file" => loggers.OfType<SerilogFileLogger>(),
                "database" => loggers.OfType<DatabaseLogger>(),
                "all" => loggers,
                _ => loggers
            };

            return new CompositeLogger(filteredLoggers, loggingConfig);
        });

        services.AddSingleton<ISmsServiceFactory, SmsServiceFactory>();
        services.AddScoped<ISmsService, MultiProviderSmsService>();

        services.AddScoped<IPushNotificationService, FirebasePushNotificationService>();

        services.AddScoped<IDeviceTokenService, DeviceTokenService>();
        services.AddScoped<IUserNotificationSettingService, UserNotificationSettingService>();
        return services;
    }

    public static IServiceCollection AddSubClassesOfType(this IServiceCollection services,Assembly assembly,Type type,
        Func<IServiceCollection, Type, IServiceCollection>? addWithLifeCycle = null
    )
    {
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type) && type != t).ToList();
        foreach (Type? item in types)
            if (addWithLifeCycle == null)
                services.AddScoped(item);
            else
                addWithLifeCycle(services, type);
        return services;
    }
}
