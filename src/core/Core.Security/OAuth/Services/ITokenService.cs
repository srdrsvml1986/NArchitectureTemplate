using NArchitectureTemplate.Core.Security.OAuth.Models;

namespace NArchitectureTemplate.Core.Security.OAuth.Services;
public interface ITokenService
{
    Task<string> GetStoredToken(string userId);
    Task StoreTokenSecurely(string userId, ExternalAuthUser externalUser);
}