using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Domain.Entities;
public class ExceptionLog : NArchitectureTemplate.Core.Security.Entities.ExceptionLog<Guid, Guid>
{
    public virtual User? User { get; set; } = null!; // Nullable User reference

}
