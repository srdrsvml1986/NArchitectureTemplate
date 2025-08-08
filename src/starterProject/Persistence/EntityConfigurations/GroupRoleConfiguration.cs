using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class GroupRoleConfiguration : IEntityTypeConfiguration<GroupRole>
{
    public void Configure(EntityTypeBuilder<GroupRole> builder)
    {
        builder.ToTable("GroupRoles").HasKey(gr => gr.Id);

        builder.Property(gr => gr.Id).HasColumnName("Id").IsRequired();
        builder.Property(gr => gr.GroupId).HasColumnName("GroupId").IsRequired();
        builder.Property(gr => gr.RoleId).HasColumnName("RoleId").IsRequired();
        builder.Property(gr => gr.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(gr => gr.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(gr => gr.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(gr => !gr.DeletedDate.HasValue);
    }
}