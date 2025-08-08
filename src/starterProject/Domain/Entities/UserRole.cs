using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

public class UserRole : NArchitecture.Core.Security.Entities.UserRole<int,Guid,int>
{
    // Navigation Properties
    public virtual User User { get; set; }
    public virtual Role Role { get; set; }
}

