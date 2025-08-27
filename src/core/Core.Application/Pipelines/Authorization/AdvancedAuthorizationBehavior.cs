using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.Types;
using NArchitectureTemplate.Core.Security.Extensions;
using System.Security.Claims;

namespace NArchitectureTemplate.Core.Application.Pipelines.Authorization;

public class AdvancedAuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IRequestAdvancedAuthorization
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly ICurrentUserAuthorizationService _authorizationService;
    private readonly Dictionary<string, List<string>> _roleHierarchy;
    private readonly Dictionary<string, List<string>> _groupHierarchy;

    public AdvancedAuthorizationBehavior(
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        ICurrentUserAuthorizationService authorizationService)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _authorizationService = authorizationService;

        // Rol hiyerarşisini yapılandırmadan yükle
        _roleHierarchy = new Dictionary<string, List<string>>();
        var roleHierarchySection = _configuration.GetSection("RoleHierarchy");
        if (roleHierarchySection.Exists())
        {
            foreach (var item in roleHierarchySection.GetChildren())
            {
                var roles = item.Get<List<string>>();
                if (roles != null)
                {
                    _roleHierarchy[item.Key] = roles;
                }
            }
        }

        // Grup hiyerarşisini yapılandırmadan yükle
        _groupHierarchy = new Dictionary<string, List<string>>();
        var groupHierarchySection = _configuration.GetSection("GroupHierarchy");
        if (groupHierarchySection.Exists())
        {
            foreach (var item in groupHierarchySection.GetChildren())
            {
                var groups = item.Get<List<string>>();
                if (groups != null)
                {
                    _groupHierarchy[item.Key] = groups;
                }
            }
        }
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
        var allUserRoles = GetAllRolesInHierarchy(userRoles);
        return requiredRoles.Any(requiredRole => allUserRoles.Contains(requiredRole));
    }

    private HashSet<string> GetAllRolesInHierarchy(IEnumerable<string> userRoles)
    {
        var allRoles = new HashSet<string>(userRoles);
        var queue = new Queue<string>(userRoles);

        while (queue.Count > 0)
        {
            var currentRole = queue.Dequeue();
            if (_roleHierarchy.TryGetValue(currentRole, out var subRoles))
            {
                foreach (var subRole in subRoles)
                {
                    if (allRoles.Add(subRole))
                    {
                        queue.Enqueue(subRole);
                    }
                }
            }
        }

        return allRoles;
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
        var allUserGroups = GetAllGroupsInHierarchy(userGroups);
        return requiredGroups.Any(requiredGroup => allUserGroups.Contains(requiredGroup));
    }

    private HashSet<string> GetAllGroupsInHierarchy(IEnumerable<string> userGroups)
    {
        var allGroups = new HashSet<string>(userGroups);
        var queue = new Queue<string>(userGroups);

        while (queue.Count > 0)
        {
            var currentGroup = queue.Dequeue();
            if (_groupHierarchy.TryGetValue(currentGroup, out var subGroups))
            {
                foreach (var subGroup in subGroups)
                {
                    if (allGroups.Add(subGroup))
                    {
                        queue.Enqueue(subGroup);
                    }
                }
            }
        }

        return allGroups;
    }
}