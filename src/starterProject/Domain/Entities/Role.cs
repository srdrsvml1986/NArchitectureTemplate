using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

public class Role : Entity<int>
{
    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<UserRole> UserRoles { get; set; }
    public ICollection<RoleOperationClaim> RoleClaims { get; set; }
    public ICollection<GroupRole> GroupRoles { get; set; }
}
