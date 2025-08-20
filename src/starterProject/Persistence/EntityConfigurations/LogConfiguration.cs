using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class LogConfiguration : IEntityTypeConfiguration<Log>
{
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.ToTable("Logs").HasKey(l => l.Id);

        builder.Property(l => l.Id).HasColumnName("Id").IsRequired();
        builder.Property(l => l.UserId).HasColumnName("UserId").IsRequired(false); // Nullable olmalý
        builder.Property(l => l.Level).HasColumnName("Level").IsRequired();
        builder.Property(l => l.Message).HasColumnName("Message").IsRequired();
        builder.Property(l => l.Timestamp).HasColumnName("Timestamp").IsRequired();
        builder.Property(l => l.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(l => l.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(l => l.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(l => l.User)
            .WithMany(u => u.Logs) // User entity'sindeki Logs koleksiyonuna baðla
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(l => !l.DeletedDate.HasValue);
    }
}