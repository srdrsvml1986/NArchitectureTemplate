using NArchitecture.Core.Application.Responses;

namespace Application.Features.UserSessions.Queries.GetById;

public class GetByIdUserSessionResponse : IResponse
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