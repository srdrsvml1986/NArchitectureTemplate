using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class UserGroup : NArchitecture.Core.Security.Entities.UserGroup<int,Guid,int>
{
    public virtual User User { get; set; }
    public virtual Group Group { get; set; }
}
