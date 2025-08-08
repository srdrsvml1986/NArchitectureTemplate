using Domain.DTos;
using Domain.Entities;
using NArchitecture.Core.Security.JWT;
using NArchitecture.Core.Security.OAuth.Models;

namespace Application.Services.AuthService;

public interface IAuthService
{
    Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken);
    Task<string> ConvertSecretKeyToString(byte[] secretKey);
    Task<AccessToken> CreateAccessToken(User user);
    Task<EmailAuthenticator> CreateEmailAuthenticator(User user);
    Task<OtpAuthenticator> CreateOtpAuthenticator(User user);
    Task<RefreshToken> CreateRefreshToken(User user, string ipAddress, string userAgent);
    Task<TokenDto> CreateTokenForExternalUser(ExternalAuthUser externalUser, string ipAddress, string userAgent);
    Task DeleteOldRefreshTokens(Guid userId);
    Task<RefreshToken?> GetRefreshTokenByToken(string token);
    Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, string ipAddress, string reason);
    Task RevokeRefreshToken(RefreshToken refreshToken, string ipAddress, string? reason = null, string? replacedByToken = null);
    Task<RefreshToken> RotateRefreshToken(User user, RefreshToken refreshToken, string ipAddress);
    Task SendAuthenticatorCode(User user);
    Task VerifyAuthenticatorCode(User user, string authenticatorCode);
    Task<int> GetRefreshCountAsync(Guid userId, TimeSpan period);
    Task<string> GetRefreshTokenBySessionAsync(Guid userId);
    Task RevokeUserSessionAsync(Guid sessionId);
    Task RevokeAllOtherSessionsAsync(Guid userId, Guid currentSessionId);
    Task FlagAndHandleSuspiciousSessionsAsync(Guid userId);
    Task<IEnumerable<UserSession>> GetActiveSessionsAsync(Guid userId);
}
