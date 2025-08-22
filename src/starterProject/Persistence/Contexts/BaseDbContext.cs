using Application.Services.EmergencyAndSecretServices;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Reflection;

namespace Persistence.Contexts;

public class BaseDbContext : DbContext
{
    protected IConfiguration Configuration { get; set; }
    public DbSet<EmailAuthenticator> EmailAuthenticators { get; set; }
    public DbSet<OperationClaim> OperationClaims { get; set; }
    public DbSet<OtpAuthenticator> OtpAuthenticators { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserOperationClaim> UserClaims { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupOperationClaim> GroupClaims { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<ResetPasswordToken> ResetPasswordTokens { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RoleOperationClaim> RoleClaims { get; set; }
    public DbSet<GroupRole> GroupRoles { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<ExceptionLog> ExceptionLogs { get; set; }

    public BaseDbContext(DbContextOptions dbContextOptions, IConfiguration configuration)
        : base(dbContextOptions)
    {
        Configuration = configuration;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    /// <summary> 
    /// Bu metod, veritabanı bağlantısını yapılandırmak için kullanılır.
    /// sadece MsSqlConfiguration ve PostgreConfiguration için kullanılmaktadır.
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var dbSettings = Configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();

            if (dbSettings == null)
                throw new InvalidOperationException("DatabaseSettings konfigürasyonu bulunamadı");

            var selectedProviderConfig = dbSettings.GetSelectedProviderConfig();

            if (dbSettings.SelectedProvider?.ToLower() == "inmemory")
            {
                optionsBuilder.UseInMemoryDatabase("InMemoryDb");
                return;
            }

            if (selectedProviderConfig == null || string.IsNullOrEmpty(selectedProviderConfig.ConnectionString))
                throw new InvalidOperationException($"Seçili provider için konfigürasyon bulunamadı: {dbSettings.SelectedProvider}");

            // Çalışma zamanında secrets manager'dan connection string al
            var connectionString = GetConnectionStringFromSecrets(dbSettings);
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string alınamadı");

            switch (dbSettings.SelectedProvider?.ToLower())
            {
                case "postgresql":
                    optionsBuilder.UseNpgsql(connectionString);
                    break;
                case "sqlserver":
                    optionsBuilder.UseSqlServer(connectionString);
                    break;
                case "oracle":
                    // Oracle için gerekli NuGet paketini ekleyin: Oracle.EntityFrameworkCore
                    optionsBuilder.UseOracle(connectionString);
                    break;
                case "mongodb":
                    // MongoDB için gerekli NuGet paketini ekleyin: MongoDB.Driver
                    // MongoDB için özel yapılandırma gerekebilir
                    ConfigureMongoDb(optionsBuilder, connectionString);
                    break;
                default:
                    throw new InvalidOperationException($"Desteklenmeyen veritabanı sağlayıcı: {dbSettings.SelectedProvider}");
            }
        }

        // PendingModelChangesWarning uyarısını bastırmak için
        optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));

    }


    /// <summary>
    /// postgreSQL için UTC zaman dilimi kullanmak üzere yapılandırma.
    /// </summary>
    /// <param name="builder"></param>
    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<DateTime>()
               .HaveConversion<UtcValueConverter>();
    }

    public class UtcValueConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcValueConverter() : base(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
        {
        }
    }
    private void ConfigureMongoDb(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        // MongoDB yapılandırması
        // Bu kısım MongoDB driver ve Entity Framework Core entegrasyonuna göre özelleştirilmeli
        var mongoUrl = new MongoUrl(connectionString);
        var mongoClient = new MongoClient(mongoUrl);
        var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);

        optionsBuilder.UseMongoDB(connectionString,mongoUrl.DatabaseName);
        // MongoDB için özel yapılandırma
        // optionsBuilder.UseMongoDb(...) gibi bir metod kullanılabilir
        // Eğer EF Core için resmi MongoDB provider'ı yoksa, bu kısmı uygun şekilde değiştirin
    }

    private string GetConnectionStringFromSecrets(DatabaseSettings dbSettings)
    {
        // Master key'i ortam değişkenlerinden al
        var masterKey = Environment.GetEnvironmentVariable("MASTER_KEY");
        if (string.IsNullOrEmpty(masterKey))
        {
            // Development ortamında default bir master key kullan
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                masterKey = "dev-master-key";
            }
            else
            {
                throw new Exception("MASTER_KEY ortam değişkeni tanımlanmalı");
            }
        }

        // Secrets manager'dan connection string'i al
        var secretsManager = new LocalSecretsManager(masterKey);

        return dbSettings.SelectedProvider?.ToLower() switch
        {
            "postgresql" => secretsManager.GetSecret("DatabaseSettings:PostgreConfiguration:ConnectionString"),
            "sqlserver" => secretsManager.GetSecret("DatabaseSettings:MsSqlConfiguration:ConnectionString"),
            "oracle" => secretsManager.GetSecret("DatabaseSettings:OracleConfiguration:ConnectionString"),
            "mongodb" => secretsManager.GetSecret("DatabaseSettings:mongodbConfiguration:ConnectionString"),
            _ => throw new InvalidOperationException($"Desteklenmeyen veritabanı sağlayıcı: {dbSettings.SelectedProvider}")
        };
    }
}
