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
        var databaseSettings = configuration.GetSection("DatabaseSettings");
        var provider = databaseSettings["Provider"];
        var connectionString = databaseSettings["ConnectionString"];

        switch (provider)
        {
            case "SqlServer" or "PostgreSql":
                services.AddDbContext<BaseDbContext>();
                break;
            case "InMemory":
                services.AddDbContext<BaseDbContext>(options =>
                    options.UseInMemoryDatabase("BaseDb"));
                break;
            default:
                throw new Exception($"Desteklenmeyen veritabanı sağlayıcı: {provider}. " +
                "Geçerli değerler: 'InMemory','SqlServer', 'PostgreSql'");
        }

        services.AddDbContext<BaseDbContext>(options => options.UseInMemoryDatabase("BaseDb"));
        services.AddDbMigrationApplier(buildServices => buildServices.GetRequiredService<BaseDbContext>());

        services.AddScoped<IEmailAuthenticatorRepository, EmailAuthenticatorRepository>();
        services.AddScoped<IOperationClaimRepository, OperationClaimRepository>();
        services.AddScoped<IOtpAuthenticatorRepository, OtpAuthenticatorRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserOperationClaimRepository, UserOperationClaimRepository>();

        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IGroupOperationClaimRepository, GroupOperationClaimRepository>();
        services.AddScoped<IUserGroupRepository, UserGroupRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IGroupOperationClaimRepository, GroupOperationClaimRepository>();
        services.AddScoped<IUserGroupRepository, UserGroupRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRoleOperationClaimRepository, RoleOperationClaimRepository>();
        services.AddScoped<IGroupRoleRepository, GroupRoleRepository>();
        services.AddScoped<ILogRepository, LogRepository>();
        services.AddScoped<IExceptionLogRepository, ExceptionLogRepository>();
        services.AddScoped<ILogRepository, LogRepository>();
        services.AddScoped<ILogRepository, LogRepository>();
        services.AddScoped<ILogRepository, LogRepository>();
        services.AddScoped<IExceptionLogRepository, ExceptionLogRepository>();
        services.AddScoped<ILogRepository, LogRepository>();
        return services;
    }
}
