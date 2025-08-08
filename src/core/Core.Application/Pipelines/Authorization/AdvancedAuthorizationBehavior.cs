using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Security.Extensions;
using System.Security.Claims;

namespace NArchitecture.Core.Application.Pipelines.Authorization;

public class AdvancedAuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IRequestAuthorization
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly ICurrentUserAuthorizationService _authorizationService;

    public AdvancedAuthorizationBehavior(
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        ICurrentUserAuthorizationService authorizationService)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _authorizationService = authorizationService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
     )
    {
        if (!_httpContextAccessor.HttpContext.User.Claims.Any())
            throw new AuthorizationException("Kimliğiniz doğrulanmadı");

        ClaimsPrincipal user = _httpContextAccessor.HttpContext.User;
        string? userIdClaim = user.GetIdClaim();

        if (string.IsNullOrEmpty(userIdClaim))
            throw new AuthorizationException("Kullanıcı kimliği bulunamadı");

        Guid userId = Guid.Parse(userIdClaim);

        var adminRole = _configuration["AuthorizationSettings:AdminRole"];
        if (string.IsNullOrEmpty(adminRole))
            throw new InvalidOperationException("AdminRole konfigürasyonda bulunamadı");

        bool isAdmin = user.IsInRole(adminRole);

        bool hasRequiredRole = !request.Roles.Any() ||
                               await HasRequiredRole(userId, request.Roles, isAdmin, cancellationToken);
        bool hasRequiredPermission = !request.Permissions.Any() ||
                                     await HasRequiredPermission(userId, request.Permissions, isAdmin, cancellationToken);
        bool hasRequiredGroup = !request.Groups.Any() ||
                                await HasRequiredGroup(userId, request.Groups, isAdmin, cancellationToken);

        if (!isAdmin && (!hasRequiredRole || !hasRequiredPermission || !hasRequiredGroup))
        {
            throw new AuthorizationException("İşlem yetkiniz yok");
        }

        return await next();
    }

    private async Task<bool> HasRequiredRole(Guid userId, IEnumerable<string> requiredRoles, bool isAdmin, CancellationToken cancellationToken)
    {
        if (isAdmin) return true;

        var userRoles = await _authorizationService.GetUserRolesAsync(userId, cancellationToken);
        return requiredRoles.Any(role => userRoles.Contains(role));
    }

    private async Task<bool> HasRequiredPermission(Guid userId, IEnumerable<string> requiredPermissions, bool isAdmin, CancellationToken cancellationToken)
    {
        if (isAdmin) return true;

        var userPermissions = await _authorizationService.GetUserPermissionsAsync(userId, cancellationToken);
        return requiredPermissions.Any(permission => userPermissions.Contains(permission));
    }

    private async Task<bool> HasRequiredGroup(Guid userId, IEnumerable<string> requiredGroups, bool isAdmin, CancellationToken cancellationToken)
    {
        if (isAdmin) return true;

        var userGroups = await _authorizationService.GetUserGroupsAsync(userId, cancellationToken);
        return requiredGroups.Any(group => userGroups.Contains(group));
    }
}
