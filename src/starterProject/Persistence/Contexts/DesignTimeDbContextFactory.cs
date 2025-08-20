using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
namespace Persistence.Contexts;


public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BaseDbContext>
{
    public BaseDbContext CreateDbContext(string[] args)
    {
        // appsettings + environment config oku
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var dbSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();
        if (dbSettings == null)
            throw new InvalidOperationException("DatabaseSettings bulunamadı.");

        var selectedProviderConfig = dbSettings.GetSelectedProviderConfig();
        if (selectedProviderConfig == null || string.IsNullOrEmpty(selectedProviderConfig.ConnectionString))
            throw new InvalidOperationException($"Seçili provider için connection string bulunamadı: {dbSettings.SelectedProvider}");

        var optionsBuilder = new DbContextOptionsBuilder<BaseDbContext>();

        // EF Core’un migration mekanizması sadece relational provider’larda tam olarak desteklenir (Npgsql, SqlServer, Oracle vs.)
        // yani MongoDB gibi NoSQL veritabanları için migration desteği yoktur.
        // MongoDB gibi NoSQL veritabanlarında Add - Migration / Update - Database işlemleri klasik anlamda kullanılmaz(çünkü tablo/ kolon kavramı yok).
        // Yani DesignTimeDbContextFactory içine MongoDB desteği koyabilirsin, fakat EF migrations çalıştırmaya kalktığında muhtemelen NotSupportedException fırlatacak.
        // Provider seçimine göre UseXxx çağır
        switch (dbSettings.SelectedProvider?.ToLower())
        {
            case "postgresql":
                optionsBuilder.UseNpgsql(selectedProviderConfig.ConnectionString,
                    b => b.MigrationsAssembly("Persistence"));
                break;
            case "sqlserver":
                optionsBuilder.UseSqlServer(selectedProviderConfig.ConnectionString,
                    b => b.MigrationsAssembly("Persistence"));
                break;
            case "oracle":
                optionsBuilder.UseOracle(selectedProviderConfig.ConnectionString,
                    b => b.MigrationsAssembly("Persistence"));
                break;
            default:
                throw new InvalidOperationException($"Desteklenmeyen provider: {dbSettings.SelectedProvider}");
        }

        return new BaseDbContext(optionsBuilder.Options, configuration);
    }
}


