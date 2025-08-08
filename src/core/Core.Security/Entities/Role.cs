using NArchitecture.Core.Persistence.Repositories;

namespace NArchitecture.Core.Security.Entities;

public class Role<TId> : Entity<TId>
{
    public string Name { get; set; }
    public string Description { get; set; }

    public Role()
    {
        Name = string.Empty;
        Description = string.Empty;
    }

    public Role(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public Role(TId id, string name, string description)
        : base(id)
    {
        Name = name;
        Description = description;
    }
}
