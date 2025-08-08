﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NArchitecture.Core.Persistence.DbMigrationApplier;

namespace NArchitecture.Core.Persistence.DependencyInjection;

public static class ServiceCollectionDbMigrationApplierExtensions
{
    public static IServiceCollection AddDbMigrationApplier<TDbContext>(
        this IServiceCollection services,
        Func<ServiceProvider, TDbContext> contextFactory
    )
        where TDbContext : DbContext
    {
        ServiceProvider buildServiceProvider = services.BuildServiceProvider();

        _ = services.AddTransient<IDbMigrationApplierService, DbMigrationApplierService<TDbContext>>(
            _ => new DbMigrationApplierService<TDbContext>(contextFactory(buildServiceProvider))
        );
        _ = services.AddTransient<IDbMigrationApplierService<TDbContext>, DbMigrationApplierService<TDbContext>>(
            _ => new DbMigrationApplierService<TDbContext>(contextFactory(buildServiceProvider))
        );

        return services;
    }
}
