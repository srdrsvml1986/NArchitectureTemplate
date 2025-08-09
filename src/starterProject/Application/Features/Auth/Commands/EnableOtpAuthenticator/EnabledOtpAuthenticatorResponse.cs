using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.Auth.Commands.EnableOtpAuthenticator;

public class EnabledOtpAuthenticatorResponse : IResponse
{
    public string SecretKey { get; set; }
    public string QrCodeImageUrl { get; set; } // Yeni eklendi
    public EnabledOtpAuthenticatorResponse()
    {
        SecretKey = string.Empty;
        QrCodeImageUrl = string.Empty; // Yeni eklendi
    }

    public EnabledOtpAuthenticatorResponse(string secretKey, string qrCodeImageUrl)
    {
        SecretKey = secretKey;
        QrCodeImageUrl = qrCodeImageUrl;
    }
}
