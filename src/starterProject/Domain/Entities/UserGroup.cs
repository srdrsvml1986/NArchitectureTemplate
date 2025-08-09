using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Domain.Entities;

public class UserGroup : NArchitectureTemplate.Core.Security.Entities.UserGroup<int,Guid,int>
{
    public virtual User User { get; set; }
    public virtual Group Group { get; set; }
}
