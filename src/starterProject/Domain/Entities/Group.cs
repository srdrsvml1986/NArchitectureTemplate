using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class Group : Entity<int>
{
    public string Name { get; set; }
    public virtual ICollection<GroupClaim> GroupClaims { get; set; }
    public virtual ICollection<UserGroup> UserGroups { get; set; }
}
