// Core/Notification/Services/INotificationService.cs
using NArchitectureTemplate.Core.Security.Entities;

namespace NArchitectureTemplate.Core.Notification.Services;

public interface INotificationService
{
    Task NotifySuspiciousSessionAsync(UserSession<Guid, Guid> session);
}