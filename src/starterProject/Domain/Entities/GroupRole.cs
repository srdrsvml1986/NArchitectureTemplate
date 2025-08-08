using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class GroupRole : NArchitecture.Core.Security.Entities.GroupRole<int,int,int>
{
    // Navigation Properties
    public virtual Group Group { get; set; }
    public virtual Role Role { get; set; }
}