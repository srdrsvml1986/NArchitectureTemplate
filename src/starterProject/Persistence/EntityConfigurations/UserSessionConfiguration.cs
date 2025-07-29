using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSessions").HasKey(us => us.Id);

        builder.Property(us => us.Id).HasColumnName("Id").IsRequired();
        builder.Property(us => us.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(us => us.IpAddress).HasColumnName("IpAddress").IsRequired();
        builder.Property(us => us.UserAgent).HasColumnName("UserAgent").IsRequired();
        builder.Property(us => us.LoginTime).HasColumnName("LoginTime").IsRequired();
        builder.Property(us => us.IsRevoked).HasColumnName("IsRevoked").IsRequired();
        builder.Property(us => us.IsSuspicious).HasColumnName("IsSuspicious").IsRequired();
        builder.Property(us => us.LocationInfo).HasColumnName("LocationInfo");
        builder.Property(us => us.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(us => us.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(us => us.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(us => !us.DeletedDate.HasValue);
    }
}