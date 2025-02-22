using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class ResetPasswordTokenConfiguration : IEntityTypeConfiguration<ResetPasswordToken>
{
    public void Configure(EntityTypeBuilder<ResetPasswordToken> builder)
    {
        builder.ToTable("ResetPasswordTokens").HasKey(rpt => rpt.Id);

        builder.Property(rpt => rpt.Id).HasColumnName("Id").IsRequired();
        builder.Property(rpt => rpt.Token).HasColumnName("Token").IsRequired();
        builder.Property(rpt => rpt.ExpirationDate).HasColumnName("ExpirationDate").IsRequired();
        builder.Property(rpt => rpt.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(rpt => rpt.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(rpt => rpt.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(rpt => rpt.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(rpt => !rpt.DeletedDate.HasValue);
    }
}