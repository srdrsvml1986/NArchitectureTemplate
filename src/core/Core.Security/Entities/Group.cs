using NArchitecture.Core.Persistence.Repositories;

namespace NArchitecture.Core.Security.Entities;

public class Group<TId> : Entity<TId>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public TId? ParentGroupId { get; set; }

    public Group()
    {
        Name = string.Empty;
    }

    public Group(string name)
    {
        Name = name;
    }

    public Group(TId id, string name, string? description = null, TId? parentGroupId = default)
        : base(id)
    {
        Name = name;
        Description = description;
        ParentGroupId = parentGroupId;
    }
}