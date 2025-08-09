namespace Domain.Entities;

public class EmailAuthenticator : NArchitectureTemplate.Core.Security.Entities.EmailAuthenticator<Guid>
{
    public virtual User User { get; set; } = default!;
}
