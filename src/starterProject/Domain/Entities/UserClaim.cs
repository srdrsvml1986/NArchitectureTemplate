namespace Domain.Entities;

public class UserClaim : NArchitecture.Core.Security.Entities.UserClaim<Guid, Guid, int>
{
    public virtual User User { get; set; } = default!;
    public virtual SecurityClaim Claim { get; set; } = default!;
}
