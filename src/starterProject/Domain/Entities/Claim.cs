using System.Security.Claims;

namespace Domain.Entities;

public class Claim : NArchitecture.Core.Security.Entities.Claim<int> 
{
    public string Description { get; set; }
    public virtual ICollection<UserClaim> UserClaims { get; set; }
    public virtual ICollection<GroupClaim> GroupClaims { get; set; }
    public virtual ICollection<RoleClaim> RoleClaims { get; set; }

}
