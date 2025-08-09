using Domain.Entities;

public class RoleOperationClaim : NArchitectureTemplate.Core.Security.Entities.RoleOperationClaim<int,int,int>
{
    // Navigation Properties
    public virtual OperationClaim OperationClaim { get; set; }
    public virtual Role Role { get; set; }
}
