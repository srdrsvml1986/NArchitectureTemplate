using NArchitecture.Core.Application.Dtos;
using Domain.Entities;

namespace Application.Features.UserSessions.Queries.GetList;

public class GetListUserSessionListItemDto : IDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string DeviceInfo { get; set; }
    public string IpAddress { get; set; }
    public DateTime LastActivity { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; }
}