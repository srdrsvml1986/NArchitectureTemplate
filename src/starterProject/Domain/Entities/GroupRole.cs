using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class GroupRole : Entity<int>
{
    public int GroupId { get; set; }
    public int RoleId { get; set; }

    // Navigation Properties
    public virtual Group Group { get; set; }
    public virtual Role Role { get; set; }
}