using System.Reflection;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL;

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

    public BaseDbContext(DbContextOptions dbContextOptions, IConfiguration configuration)
        : base(dbContextOptions)
    {
        Configuration = configuration;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    ///// <summary> 
    ///// Bu metod, veritabanı bağlantısını yapılandırmak için kullanılır.
    ///// sadece MsSqlConfiguration ve PostgreConfiguration için kullanılmaktadır.
    ///// </summary>
    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    // burası önemli
    //    //base.OnConfiguring(optionsBuilder);
    //    var cn = Configuration.GetSection("SeriLogConfigurations")
    //             .GetSection("MsSqlConfiguration")
    //             .GetSection("ConnectionString");

    //    if (!optionsBuilder.IsConfigured)
    //    {
    //        optionsBuilder.UseSqlServer(cn.Value);
    //          //optionsBuilder.UseNpgsql(cn.Value);
    //    }

    //    // PendingModelChangesWarning uyarısını bastırmak için
    //    optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    //    optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);


    //}


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
}
