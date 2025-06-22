using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

public class Role : Entity<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int? ParentRoleId { get; set; }

    // Navigation Properties
    public virtual Role ParentRole { get; set; }
    public virtual ICollection<Role> ChildRoles { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<RolePermission> RolePermissions { get; set; }
}
