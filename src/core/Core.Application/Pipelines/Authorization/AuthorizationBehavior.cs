using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Security.Constants;
using NArchitecture.Core.Security.Extensions;

namespace NArchitecture.Core.Application.Pipelines.Authorization;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ISecuredRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    // Yapılandırma dosyasına eklenmesi gereken ayar:
    // "AuthorizationSettings": { "AdminRole": "Admin" }
    private readonly AuthorizationSettings _authorizationSettings;

    public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _authorizationSettings = configuration.GetSection("AuthorizationSettings").Get<AuthorizationSettings>();
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (!_httpContextAccessor.HttpContext.User.Claims.Any())
            throw new AuthorizationException("Kimliğiniz doğrulanmadı");

        if (request.Roles.Any())
        {
            ICollection<string> userRoleClaims = _httpContextAccessor.HttpContext.User.GetRoleClaims() ?? [];
            bool hasMatchingRole = userRoleClaims.Intersect(request.Roles).Any();
            bool isAdmin = userRoleClaims.Contains(_authorizationSettings.AdminRole);

            if (!hasMatchingRole && !isAdmin)
                throw new AuthorizationException("İşlem yetkiniz yok");
        }


        TResponse response = await next();
        return response;
    }
}
