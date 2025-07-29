using Domain.DTos;


namespace Application.Features.UserSessions.Queries.GetActiveSessions;
public class GetActiveSessionsResponse
{
    public Guid UserId { get; set; }
    public List<UserSessionDto> Sessions { get; set; } = new();
}