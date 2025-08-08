using Microsoft.Extensions.Options;
using NArchitecture.Core.Security.OAuth.Configurations;
using NArchitecture.Core.Security.OAuth.Models;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace NArchitecture.Core.Security.OAuth.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly GoogleAuthConfig _googleAuthConfig;

    public GoogleAuthService(IOptions<GoogleAuthConfig> googleAuthConfig)
    {
        _googleAuthConfig = googleAuthConfig.Value;
    }

    public string GetAuthorizationUrl()
    {
        return $"https://accounts.google.com/o/oauth2/auth?client_id={_googleAuthConfig.ClientId}&redirect_uri={_googleAuthConfig.RedirectUri}&response_type=code&scope=email%20profile";
    }

    public async Task<OAuthResponse> AuthenticateAsync(string code)
    {
        // Google token alma işlemi
        using var httpClient = new HttpClient();
        var tokenResponse = await httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("client_id", _googleAuthConfig.ClientId),
            new KeyValuePair<string, string>("client_secret", _googleAuthConfig.ClientSecret),
            new KeyValuePair<string, string>("redirect_uri", _googleAuthConfig.RedirectUri),
            new KeyValuePair<string, string>("grant_type", "authorization_code")
        }));

        if (!tokenResponse.IsSuccessStatusCode)
            return new OAuthResponse { Success = false, Error = "Google token alınamadı." };
        var content = await tokenResponse.Content.ReadAsStringAsync();
        var tokenData = await tokenResponse.Content.ReadFromJsonAsync<GoogleTokenResponse>();

        // Kullanıcı bilgilerini alma
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenData?.AccessToken);
        var userInfoResponse = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");

        if (!userInfoResponse.IsSuccessStatusCode)
            return new OAuthResponse { Success = false, Error = "Google kullanıcı bilgileri alınamadı." };

        var userInfo = await userInfoResponse.Content.ReadFromJsonAsync<GoogleUserInfo>();

        return new OAuthResponse
        {
            Success = true,
            User = new ExternalAuthUser
            {
                Id = userInfo?.Id ?? string.Empty,
                Email = userInfo?.Email ?? string.Empty,
                FirstName = userInfo?.Name ?? string.Empty,
                AccessToken = tokenData?.AccessToken ?? string.Empty,
                Provider = "Google"
            }
        };
    }
}

// Google token yanıt modeli
// Google token yanıt modeli

public class GoogleTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;

    [JsonPropertyName("scope")]
    public string Scope { get; set; } = string.Empty;

    [JsonPropertyName("id_token")]
    public string IdToken { get; set; } = string.Empty;
}


// Google kullanıcı bilgileri modeli
public class GoogleUserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
