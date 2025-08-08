using NArchitecture.Core.Application.Responses;

namespace Application.Features.UserSessions.Commands.RevokeUserSession;

public class RevokeMySessionResponse : IResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string IpAddress { get; set; }
    public DateTime LoginTime { get; set; }
    public bool IsRevoked { get; set; }
    public string Message { get; set; } = "Session revoked successfully";
}