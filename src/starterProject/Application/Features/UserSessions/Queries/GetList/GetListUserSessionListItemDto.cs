using NArchitecture.Core.Application.Dtos;
using Domain.Entities;

namespace Application.Features.UserSessions.Queries.GetList;

public class GetListUserSessionListItemDto : IDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime LoginTime { get; set; }
    public bool IsRevoked { get; set; }
    public bool IsSuspicious { get; set; }
    public string? LocationInfo { get; set; }
}