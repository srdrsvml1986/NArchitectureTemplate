using Microsoft.Extensions.Caching.Distributed;
using NArchitectureTemplate.Core.Security.Encryption;
using NArchitectureTemplate.Core.Security.OAuth.Models;

namespace NArchitectureTemplate.Core.Security.OAuth.Services;
public class TokenService : ITokenService
{
    private readonly IDistributedCache _cache;
    public async Task StoreTokenSecurely(string userId, ExternalAuthUser externalUser)
    {
        try
        {
            // Token'ı şifreleyerek sakla
            var encryptedToken = EncryptionHelper.Encrypt(externalUser.AccessToken);

            // Distributed cache'de sakla
            await _cache.SetStringAsync(
                $"oauth_token_{userId}",
                encryptedToken,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                }
            );
        }
        catch (Exception ex)
        {
            throw new Exception("Token encryption and storage failed", ex);
        }
    }

    public async Task<string> GetStoredToken(string userId)
    {
        try
        {
            var encryptedToken = await _cache.GetStringAsync($"oauth_token_{userId}");
            if (string.IsNullOrEmpty(encryptedToken))
                return string.Empty;

            return EncryptionHelper.Decrypt(encryptedToken);
        }
        catch (Exception ex)
        {
            throw new Exception("Token retrieval and decryption failed", ex);
        }
    }

}
