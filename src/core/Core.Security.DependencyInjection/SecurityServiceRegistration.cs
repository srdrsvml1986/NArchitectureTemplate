using Microsoft.Extensions.DependencyInjection;
using NArchitectureTemplate.Core.Security.EmailAuthenticator;
using NArchitectureTemplate.Core.Security.JWT;
using NArchitectureTemplate.Core.Security.OtpAuthenticator;
using NArchitectureTemplate.Core.Security.OtpAuthenticator.OtpNet;

namespace NArchitectureTemplate.Core.Security.DependencyInjection;

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
