namespace Domain.Entities;

public class UserOperationClaim : NArchitectureTemplate.Core.Security.Entities.UserOperationClaim<Guid, Guid, int>
{
    public virtual User User { get; set; } = default!;
    public virtual OperationClaim OperationClaim { get; set; } = default!;
}
