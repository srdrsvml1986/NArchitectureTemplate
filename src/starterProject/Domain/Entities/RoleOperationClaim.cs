using Domain.Entities;

public class RoleOperationClaim : NArchitecture.Core.Security.Entities.RoleOperationClaim<int,int,int>
{
    // Navigation Properties
    public virtual OperationClaim OperationClaim { get; set; }
    public virtual Role Role { get; set; }
}
