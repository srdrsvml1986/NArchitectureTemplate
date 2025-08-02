using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

public class Role : Entity<int>
{
    public required string Name { get; set; }
    public required string Description { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<RoleOperationClaim> RoleOperationClaims { get; set; }
    public virtual ICollection<GroupRole> GroupRoles { get; set; }
}
