using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.ToTable("RoleClaims").HasKey(rc => rc.Id);

        builder.Property(rc => rc.Id).HasColumnName("Id").IsRequired();
        builder.Property(rc => rc.ClaimId).HasColumnName("ClaimId").IsRequired();
        builder.Property(rc => rc.RoleId).HasColumnName("RoleId").IsRequired();
        builder.Property(rc => rc.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(rc => rc.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(rc => rc.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(rc => !rc.DeletedDate.HasValue);
    }
}