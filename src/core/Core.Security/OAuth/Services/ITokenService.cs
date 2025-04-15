using NArchitecture.Core.Security.OAuth.Models;

namespace NArchitecture.Core.Security.OAuth.Services;
public interface ITokenService
{
    Task<string> GetStoredToken(string userId);
    Task StoreTokenSecurely(string userId, ExternalAuthUser externalUser);
}