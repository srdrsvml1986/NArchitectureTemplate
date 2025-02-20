using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NArchitecture.Core.Security.Hashing;

namespace Persistence.EntityConfigurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("Groups").HasKey(g => g.Id);

        builder.Property(g => g.Id).HasColumnName("Id").IsRequired();
        builder.Property(g => g.Name).HasColumnName("Name").IsRequired();
        builder.Property(g => g.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(g => g.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(g => g.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(g => !g.DeletedDate.HasValue);
        builder.HasData(_seeds);

        builder.HasBaseType((string)null!);
    }

    public static int GroupId { get; } = 1;
    private IEnumerable<Group> _seeds
    {
        get
        {
            Group[] groups= new[]{ 
                new Group {
                Id = GroupId,
                Name = "Admin Group"},  
                new Group {
                Id = 2,
                Name = "User Group"}         
            };

            foreach (var group in groups)
                yield return group;
        }
    }
}
