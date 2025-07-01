using Microsoft.AspNetCore.Identity;
using NArchitecture.Core.Persistence.Repositories;

namespace NArchitecture.Core.Security.Entities;

public class Claim<TId> : Entity<TId>
{
    public string Name { get; set; }

    public Claim()
    {
        Name = string.Empty;
    }

    public Claim(string name)
    {
        Name = name;
    }

    public Claim(TId id, string name)
        : base(id)
    {
        Name = name;
    }
}
