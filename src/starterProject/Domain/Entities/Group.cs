using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class Group : Entity<int>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public virtual ICollection<GroupOperationClaim> GroupOperationClaims { get; set; }
    public virtual ICollection<GroupRole> GroupRoles { get; set; }
    public virtual ICollection<UserGroup> UserGroups { get; set; }
}
