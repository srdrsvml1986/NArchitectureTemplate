using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleOperationClaim>
{
    public void Configure(EntityTypeBuilder<RoleOperationClaim> builder)
    {
        builder.ToTable("RoleOperationClaims").HasKey(rc => rc.Id);

        builder.Property(rc => rc.Id).HasColumnName("Id").IsRequired();
        builder.Property(rc => rc.OperationClaimId).HasColumnName("OperationClaimId").IsRequired();
        builder.Property(rc => rc.RoleId).HasColumnName("RoleId").IsRequired();
        builder.Property(rc => rc.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(rc => rc.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(rc => rc.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(rc => !rc.DeletedDate.HasValue);
    }
}