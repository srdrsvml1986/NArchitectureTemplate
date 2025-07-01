using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class GroupClaimConfiguration : IEntityTypeConfiguration<GroupClaim>
{
    public void Configure(EntityTypeBuilder<GroupClaim> builder)
    {
        builder.ToTable("GroupClaims").HasKey(gc => gc.Id);

        builder.Property(gc => gc.Id).HasColumnName("Id").IsRequired();
        builder.Property(gc => gc.ClaimId).HasColumnName("ClaimId").IsRequired();
        builder.Property(gc => gc.GroupId).HasColumnName("GroupId").IsRequired();
        builder.Property(gc => gc.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(gc => gc.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(gc => gc.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(uoc => uoc.Group);
        builder.HasOne(uoc => uoc.Claim);

        builder.HasQueryFilter(gc => !gc.DeletedDate.HasValue);
        builder.HasData(_seeds);

        builder.HasBaseType((string)null!);
    }

    public static int GroupClaimId { get; } = 1;
    private IEnumerable<GroupClaim> _seeds
    {
        get
        {
            yield return new()
            {
                Id = GroupClaimId,
                GroupId = GroupConfiguration.GroupId,
                ClaimId = ClaimConfiguration.AdminId
            };
        }
    }
}
