using System.Reflection;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence.Contexts;

public class BaseDbContext : DbContext
{
    protected IConfiguration Configuration { get; set; }
    public DbSet<EmailAuthenticator> EmailAuthenticators { get; set; }
    public DbSet<Claim> OperationClaims { get; set; }
    public DbSet<OtpAuthenticator> OtpAuthenticators { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserClaim> UserOperationClaims { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupClaim> GroupClaims { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<ResetPasswordToken> ResetPasswordTokens { get; set; }

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
    ///// Bu metot, veritabanı bağlantısını yapılandırmak için kullanılır.
    ///// sadece MsSqlConfiguration için kullanılmaktadır.
    ///// </summary>
    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    // TODO 
    //    // burası önemli
    //    //base.OnConfiguring(optionsBuilder);
    //    var cn = Configuration.GetSection("SeriLogConfigurations")
    //             .GetSection("MsSqlConfiguration")
    //             .GetSection("ConnectionString");

    //    if (!optionsBuilder.IsConfigured)
    //    {
    //        optionsBuilder.UseSqlServer(cn.Value);
    //    }

    //    optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

    //    // PendingModelChangesWarning uyarısını bastırmak için
    //    optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));

    //}


}
