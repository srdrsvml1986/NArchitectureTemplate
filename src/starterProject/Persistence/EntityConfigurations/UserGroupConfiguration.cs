using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> builder)
    {
        builder.ToTable("UserGroups").HasKey(ug => ug.Id);

        builder.Property(ug => ug.Id).HasColumnName("Id").IsRequired();
        builder.Property(ug => ug.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(ug => ug.GroupId).HasColumnName("GroupId").IsRequired();
        builder.Property(ug => ug.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(ug => ug.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(ug => ug.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(uoc => uoc.User);
        builder.HasOne(uoc => uoc.Group);

        builder.HasQueryFilter(ug => !ug.DeletedDate.HasValue);
        builder.HasData(_seeds);

        builder.HasBaseType((string)null!);
    }

    public static int UserGroupId { get; } = 1;
    private IEnumerable<UserGroup> _seeds
    {
        get
        {
            yield return new()
            {
                Id = UserGroupId,
                GroupId = GroupConfiguration.GroupId,
                UserId = UserConfiguration.AdminId,                
            };
        }
    }
}
