using NArchitecture.Core.Application.Responses;

namespace Application.Features.UserSessions.Queries.GetActiveSessionCount;

// GetActiveSessionCountResponse
public class GetMyActiveSessionCountResponse : IResponse
{
    public Guid UserId { get; set; }
    public int ActiveSessionCount { get; set; }
}
