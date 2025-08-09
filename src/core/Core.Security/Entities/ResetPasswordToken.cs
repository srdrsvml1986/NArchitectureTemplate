using NArchitectureTemplate.Core.Persistence.Repositories;

namespace NArchitectureTemplate.Core.Security.Entities;

public class ResetPasswordToken<TId, TUserId> : Entity<TId>
{
    public required string Token { get; set; }
    public DateTime ExpirationDate { get; set; }
    public TUserId UserId { get; set; }

    public ResetPasswordToken()
    {
        Token = string.Empty;
        UserId = default!;
    }

    public ResetPasswordToken(string token, DateTime expirationDate, TUserId userId)
    {
        Token = token;
        ExpirationDate = expirationDate;
        UserId = userId;
    }

    public ResetPasswordToken(TId id, string token, DateTime expirationDate, TUserId userId)
        : base(id)
    {
        Token = token;
        ExpirationDate = expirationDate;
        UserId = userId;
    }
}
