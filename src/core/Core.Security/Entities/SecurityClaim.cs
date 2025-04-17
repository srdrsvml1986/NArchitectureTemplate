using NArchitecture.Core.Persistence.Repositories;

namespace NArchitecture.Core.Security.Entities;

public class SecurityClaim<TId> : Entity<TId>
{
    public string Name { get; set; }

    public SecurityClaim()
    {
        Name = string.Empty;
    }

    public SecurityClaim(string name)
    {
        Name = name;
    }

    public SecurityClaim(TId id, string name)
        : base(id)
    {
        Name = name;
    }
}
