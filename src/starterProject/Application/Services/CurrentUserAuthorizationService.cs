using Application.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using NArchitecture.Core.Application.Pipelines.Authorization;

namespace Application.Services;

using Application.Services.Repositories;

public class CurrentUserAuthorizationService : ICurrentUserAuthorizationService
{
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IUserOperationClaimRepository _userOperationClaimRepository;
    private readonly IGroupOperationClaimRepository _groupOperationClaimRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IGroupRoleRepository _groupRoleRepository;

    public CurrentUserAuthorizationService(
        IUserGroupRepository userGroupRepository,
        IUserOperationClaimRepository userOperationClaimRepository,
        IGroupOperationClaimRepository groupOperationClaimRepository,
        IUserRoleRepository userRoleRepository,
        IGroupRoleRepository groupRoleRepository)
    {
        _userGroupRepository = userGroupRepository;
        _userOperationClaimRepository = userOperationClaimRepository;
        _groupOperationClaimRepository = groupOperationClaimRepository;
        _userRoleRepository = userRoleRepository;
        _groupRoleRepository = groupRoleRepository;
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var directRoles = (await _userRoleRepository.GetListAsync(
            predicate: ur => ur.UserId == userId,
            include: ur => ur.Include(ur => ur.Role),
            cancellationToken: cancellationToken
        )).Items.Select(ur => ur.Role.Name);

        var groupRoles = (await _groupRoleRepository.GetListAsync(
            predicate: gr => gr.Group.UserGroups.Any(ug => ug.UserId == userId),
            include: gr => gr.Include(gr => gr.Role),
            cancellationToken: cancellationToken
        )).Items.Select(gr => gr.Role.Name);

        return directRoles.Concat(groupRoles).Distinct();
    }

    public async Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var directPermissions = (await _userOperationClaimRepository.GetListAsync(
            predicate: uoc => uoc.UserId == userId,
            include: uoc => uoc.Include(uoc => uoc.OperationClaim),
            cancellationToken: cancellationToken
        )).Items.Select(uoc => uoc.OperationClaim.Name);

        var groupPermissions = (await _groupOperationClaimRepository.GetListAsync(
            predicate: goc => goc.Group.UserGroups.Any(ug => ug.UserId == userId),
            include: goc => goc.Include(goc => goc.OperationClaim),
            cancellationToken: cancellationToken
        )).Items.Select(goc => goc.OperationClaim.Name);

        return directPermissions.Concat(groupPermissions).Distinct();
    }

    public async Task<IEnumerable<string>> GetUserGroupsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userGroups = await _userGroupRepository.GetListAsync(
            predicate: ug => ug.UserId == userId,
            include: ug => ug.Include(ug => ug.Group),
            cancellationToken: cancellationToken
        );

        return userGroups.Items.Select(ug => ug.Group.Name).Distinct();
    }
}
