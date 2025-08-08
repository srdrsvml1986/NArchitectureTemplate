using System.Security.Claims;

namespace Domain.Entities;

public class OperationClaim : NArchitecture.Core.Security.Entities.OperationClaim<int> 
{
    public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; }
    public virtual ICollection<GroupOperationClaim> GroupOperationClaims { get; set; }
    public virtual ICollection<RoleOperationClaim> RoleOperationClaims { get; set; }

}
