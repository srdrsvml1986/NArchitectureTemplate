using NArchitecture.Core.Security.OAuth.Models;

namespace NArchitecture.Core.Security.OAuth.Services;
public interface IFacebookAuthService
{
    Task<OAuthResponse> AuthenticateAsync(string code);
    string GetAuthorizationUrl();
}
