using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities;

public class UserGroup : Entity<int>
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public int GroupId { get; set; }
    public virtual Group Group { get; set; }
}
