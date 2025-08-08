using Microsoft.Extensions.DependencyInjection;
using NArchitecture.Core.Security.EmailAuthenticator;
using NArchitecture.Core.Security.JWT;
using NArchitecture.Core.Security.OtpAuthenticator;
using NArchitecture.Core.Security.OtpAuthenticator.OtpNet;

namespace NArchitecture.Core.Security.DependencyInjection;

public static class SecurityServiceRegistration
{
    public static IServiceCollection AddSecurityServices<TUserId, TOperationClaimId, TRoleId, TRefreshTokenId>(
        this IServiceCollection services,
        TokenOptions tokenOptions
    )
    {
        services.AddScoped<
            ITokenHelper<TUserId, TOperationClaimId, TRoleId, TRefreshTokenId>,
            JwtHelper<TUserId, TOperationClaimId,TRoleId, TRefreshTokenId>
        >(_ => new JwtHelper<TUserId, TOperationClaimId, TRoleId, TRefreshTokenId>(tokenOptions));
        services.AddScoped<IEmailAuthenticatorHelper, EmailAuthenticatorHelper>();
        services.AddScoped<IOtpAuthenticatorHelper, OtpNetOtpAuthenticatorHelper>();

        return services;
    }
}
