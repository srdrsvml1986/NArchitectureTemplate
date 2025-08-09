using NArchitectureTemplate.Core.Security.OAuth.Models;

namespace NArchitectureTemplate.Core.Security.OAuth.Services;
public interface IFacebookAuthService
{
    Task<OAuthResponse> AuthenticateAsync(string code);
    string GetAuthorizationUrl();
}
