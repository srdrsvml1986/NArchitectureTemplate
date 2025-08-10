using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Domain.Entities;
public class Role : NArchitectureTemplate.Core.Security.Entities.Role<int>
{
    public ICollection<UserRole> UserRoles { get; set; }
    public ICollection<RoleOperationClaim> RoleOperationClaims { get; set; }
    public ICollection<GroupRole> GroupRoles { get; set; }
}
