namespace Domain.Entities;

public class UserClaim : NArchitecture.Core.Security.Entities.UserClaim<Guid, Guid, int>
{
    public virtual User User { get; set; } = default!;
    public virtual Claim Claim { get; set; } = default!;
}
