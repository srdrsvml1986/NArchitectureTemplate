using Microsoft.AspNetCore.Identity;
using NArchitectureTemplate.Core.Persistence.Repositories;

namespace NArchitectureTemplate.Core.Security.Entities;

public class OperationClaim<TId> : Entity<TId>
{
    public string Name { get; set; }

    public OperationClaim()
    {
        Name = string.Empty;
    }

    public OperationClaim(string name)
    {
        Name = name;
    }

    public OperationClaim(TId id, string name)
        : base(id)
    {
        Name = name;
    }
}
