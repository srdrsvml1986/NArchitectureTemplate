using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Domain.Entities;
public class Log : NArchitectureTemplate.Core.Security.Entities.Log<int>
{
  public virtual User? User { get; set; } // Nullable User reference
}
