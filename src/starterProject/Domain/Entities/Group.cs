using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Domain.Entities;

public class Group : NArchitectureTemplate.Core.Security.Entities.Group<int>
{
    public virtual ICollection<GroupOperationClaim> GroupOperationClaims { get; set; }
    public virtual ICollection<GroupRole> GroupRoles { get; set; }
    public virtual ICollection<UserGroup> UserGroups { get; set; }
}
