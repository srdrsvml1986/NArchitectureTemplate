using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

public class Role : NArchitecture.Core.Security.Entities.Role<int>
{
    public ICollection<UserRole> UserRoles { get; set; }
    public ICollection<RoleOperationClaim> RoleOperationClaims { get; set; }
    public ICollection<GroupRole> GroupRoles { get; set; }
}
