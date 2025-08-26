using NArchitectureTemplate.Core.Persistence.Repositories;
using System.ComponentModel.DataAnnotations;

namespace NArchitectureTemplate.Core.Security.Entities;

public class DeviceToken<TId,TUserId> : Entity<TId>
{
    [Required]
    public TUserId UserId { get; set; }

    [Required]
    public string Token { get; set; }

    public string DeviceType { get; set; } // iOS, Android, Web
    public string DeviceName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation property

    public DeviceToken()
    {
        Token = string.Empty;
        DeviceType = string.Empty;
        DeviceName = string.Empty;
    }

    public DeviceToken(TUserId userId, string token, string deviceType, string deviceName)
    {
        UserId = userId;
        Token = token;
        DeviceType = deviceType;
        DeviceName = deviceName;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public DeviceToken(TId id, TUserId userId, string token, string deviceType, string deviceName) : base(id)
    {
        UserId = userId;
        Token = token;
        DeviceType = deviceType;
        DeviceName = deviceName;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }
}
