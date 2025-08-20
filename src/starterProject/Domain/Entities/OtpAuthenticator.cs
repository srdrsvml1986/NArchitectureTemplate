namespace Domain.Entities;

public class OtpAuthenticator : NArchitectureTemplate.Core.Security.Entities.OtpAuthenticator<Guid, Guid>
{
    public virtual User User { get; set; } = default!;
}
