using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Domain.Entities;
public class Log : NArchitectureTemplate.Core.Security.Entities.Log<Guid, Guid?>
{
  public virtual User? User { get; set; } = null!; // Nullable User reference
}
