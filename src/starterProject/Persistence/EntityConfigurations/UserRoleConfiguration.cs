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
        builder.HasData(_seeds);
        builder.HasQueryFilter(ur => !ur.DeletedDate.HasValue);
    }
    public static int UserRoleId { get; } = 1;
    private IEnumerable<UserRole> _seeds
    {
        get
        {
            yield return new()
            {
                Id = UserRoleId,
                RoleId = 1,
                UserId = UserConfiguration.AdminId,
            };
        }
    }
}