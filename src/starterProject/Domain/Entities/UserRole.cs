using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Repositories;

public class UserRole : NArchitectureTemplate.Core.Security.Entities.UserRole<int,Guid,int>
{
    // Navigation Properties
    public virtual User User { get; set; }
    public virtual Role Role { get; set; }
}

