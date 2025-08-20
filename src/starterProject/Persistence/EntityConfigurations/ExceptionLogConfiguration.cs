using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class ExceptionLogConfiguration : IEntityTypeConfiguration<ExceptionLog>
{
    public void Configure(EntityTypeBuilder<ExceptionLog> builder)
    {
        builder.ToTable("ExceptionLogs").HasKey(el => el.Id);

        builder.Property(el => el.Id).HasColumnName("Id").IsRequired();
        builder.Property(el => el.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(el => el.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(el => el.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(el => el.User)
        .WithMany(u => u.ExceptionLogs) // User entity'sindeki ExceptionLogs koleksiyonuna baðla
        .HasForeignKey(el => el.UserId)
        .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(el => !el.DeletedDate.HasValue);
    }
}