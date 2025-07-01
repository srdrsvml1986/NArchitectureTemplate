using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles").HasKey(ur => ur.Id);

        builder.Property(ur => ur.Id).HasColumnName("Id").IsRequired();
        builder.Property(ur => ur.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(ur => ur.RoleId).HasColumnName("RoleId").IsRequired();
        builder.Property(ur => ur.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(ur => ur.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(ur => ur.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(ur => !ur.DeletedDate.HasValue);
    }
}