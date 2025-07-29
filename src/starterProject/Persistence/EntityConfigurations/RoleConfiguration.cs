using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles").HasKey(r => r.Id);

        builder.Property(r => r.Id).HasColumnName("Id").IsRequired();
        builder.Property(r => r.Name).HasColumnName("Name").IsRequired();
        builder.Property(r => r.Description).HasColumnName("Description").IsRequired();
        builder.Property(r => r.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(r => r.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(r => r.DeletedDate).HasColumnName("DeletedDate");
        builder.HasData(
            new Role
            {
                Id = 1,
                Name = "Admin",
                Description = "Tüm sistem yetkilerine sahiptir"
            },
            new Role
            {
                Id = 2,
                Name = "Service",
                Description = "Yönetici yetkilerine sahiptir"
            },
            new Role
            {
                Id = 3,
                Name = "Employee",
                Description = "Temel kullanýcý yetkilerine sahiptir"
            }
        );
        builder.HasQueryFilter(r => !r.DeletedDate.HasValue);
    }
}