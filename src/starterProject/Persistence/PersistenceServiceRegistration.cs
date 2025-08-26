using Application.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NArchitectureTemplate.Core.Persistence.DependencyInjection;
using Persistence.Contexts;
using Persistence.Repositories;

namespace Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var dbSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();

        if (dbSettings == null)
            throw new InvalidOperationException("DatabaseSettings konfigürasyonu bulunamadı");

        var selectedProviderConfig = dbSettings.SelectedProvider;

        if (selectedProviderConfig != null && selectedProviderConfig == "InMemory")
        {
            services.AddDbContext<BaseDbContext>(options => options.UseInMemoryDatabase("BaseDb"));
        }
        else if (selectedProviderConfig != null)
        {
            services.AddDbContext<BaseDbContext>();
        }

        services.AddDbMigrationApplier(buildServices => buildServices.GetRequiredService<BaseDbContext>());

        services.AddScoped<IEmailAuthenticatorRepository, EmailAuthenticatorRepository>();
        services.AddScoped<IOperationClaimRepository, OperationClaimRepository>();
        services.AddScoped<IOtpAuthenticatorRepository, OtpAuthenticatorRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserOperationClaimRepository, UserOperationClaimRepository>();

        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IUserGroupRepository, UserGroupRepository>();
        services.AddScoped<IGroupOperationClaimRepository, GroupOperationClaimRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRoleOperationClaimRepository, RoleOperationClaimRepository>();
        services.AddScoped<IGroupRoleRepository, GroupRoleRepository>();
        services.AddScoped<IExceptionLogRepository, ExceptionLogRepository>();
        services.AddScoped<ILogRepository, LogRepository>();
        services.AddScoped<IDeviceTokenRepository, DeviceTokenRepository>();
        return services;
    }
}
