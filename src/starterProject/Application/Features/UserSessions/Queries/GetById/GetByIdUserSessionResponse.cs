using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.UserSessions.Queries.GetById;

public class GetByIdUserSessionResponse : IResponse
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