using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

public class UserSession : Entity<Guid>
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string DeviceInfo { get; set; }
    public string IpAddress { get; set; }
    public DateTime LastActivity { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; }

    // Navigation Property
    public virtual User User { get; set; }
}
