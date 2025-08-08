using Microsoft.Extensions.Options;
using NArchitecture.Core.Security.OAuth.Configurations;
using NArchitecture.Core.Security.OAuth.Models;
using System.Net.Http.Json;

namespace NArchitecture.Core.Security.OAuth.Services;

public class FacebookAuthService : IFacebookAuthService
{
    private readonly FacebookAuthConfig _facebookAuthConfig;

    public FacebookAuthService(IOptions<FacebookAuthConfig> facebookAuthConfig)
    {
        _facebookAuthConfig = facebookAuthConfig.Value;
    }

    public string GetAuthorizationUrl()
    {
        return $"https://www.facebook.com/v12.0/dialog/oauth?client_id={_facebookAuthConfig.AppId}&redirect_uri={_facebookAuthConfig.RedirectUri}&response_type=code&scope=email%20public_profile";
    }

    public async Task<OAuthResponse> AuthenticateAsync(string code)
    {
        using var httpClient = new HttpClient();

        // Access token alma
        var tokenResponse = await httpClient.GetAsync(
            $"https://graph.facebook.com/v12.0/oauth/access_token?" +
            $"client_id={_facebookAuthConfig.AppId}&" +
            $"client_secret={_facebookAuthConfig.AppSecret}&" +
            $"redirect_uri={_facebookAuthConfig.RedirectUri}&" +
            $"code={code}");

        if (!tokenResponse.IsSuccessStatusCode)
            return new OAuthResponse { Success = false, Error = "Facebook token alınamadı." };

        var tokenData = await tokenResponse.Content.ReadFromJsonAsync<FacebookTokenResponse>();

        // Kullanıcı bilgilerini alma
        var userInfoResponse = await httpClient.GetAsync(
            $"https://graph.facebook.com/me?" +
            $"fields=id,name,email&" +
            $"access_token={tokenData?.AccessToken}");

        if (!userInfoResponse.IsSuccessStatusCode)
            return new OAuthResponse { Success = false, Error = "Facebook kullanıcı bilgileri alınamadı." };

        var userInfo = await userInfoResponse.Content.ReadFromJsonAsync<FacebookUserInfo>();

        return new OAuthResponse
        {
            Success = true,
            User = new ExternalAuthUser
            {
                Id = userInfo?.Id ?? string.Empty,
                Email = userInfo?.Email ?? string.Empty,
                FirstName = userInfo?.Name ?? string.Empty,
                Provider = "Facebook",
                AccessToken = tokenData?.AccessToken ?? string.Empty
            }
        };
    }
}

// Facebook token yanıt modeli
public class FacebookTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
}

// Facebook kullanıcı bilgileri modeli
public class FacebookUserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
