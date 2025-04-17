namespace Domain.Entities;

public class UserSecurityClaim : NArchitecture.Core.Security.Entities.UserSecurityClaim<Guid, Guid, int>
{
    public virtual User User { get; set; } = default!;
    public virtual SecurityClaim Claim { get; set; } = default!;
}
