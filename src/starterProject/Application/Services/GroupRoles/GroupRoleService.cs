using Application.Features.GroupRoles.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.GroupRoles;

public class GroupRoleService : IGroupRoleService
{
    private readonly IGroupRoleRepository _groupRoleRepository;
    private readonly GroupRoleBusinessRules _groupRoleBusinessRules;

    public GroupRoleService(IGroupRoleRepository groupRoleRepository, GroupRoleBusinessRules groupRoleBusinessRules)
    {
        _groupRoleRepository = groupRoleRepository;
        _groupRoleBusinessRules = groupRoleBusinessRules;
    }

    public async Task<GroupRole?> GetAsync(
        Expression<Func<GroupRole, bool>> predicate,
        Func<IQueryable<GroupRole>, IIncludableQueryable<GroupRole, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        GroupRole? groupRole = await _groupRoleRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return groupRole;
    }

    public async Task<IPaginate<GroupRole>?> GetListAsync(
        Expression<Func<GroupRole, bool>>? predicate = null,
        Func<IQueryable<GroupRole>, IOrderedQueryable<GroupRole>>? orderBy = null,
        Func<IQueryable<GroupRole>, IIncludableQueryable<GroupRole, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<GroupRole> groupRoleList = await _groupRoleRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return groupRoleList;
    }

    public async Task<GroupRole> AddAsync(GroupRole groupRole)
    {
        GroupRole addedGroupRole = await _groupRoleRepository.AddAsync(groupRole);

        return addedGroupRole;
    }

    public async Task<GroupRole> UpdateAsync(GroupRole groupRole)
    {
        GroupRole updatedGroupRole = await _groupRoleRepository.UpdateAsync(groupRole);

        return updatedGroupRole;
    }

    public async Task<GroupRole> DeleteAsync(GroupRole groupRole, bool permanent = false)
    {
        GroupRole deletedGroupRole = await _groupRoleRepository.DeleteAsync(groupRole);

        return deletedGroupRole;
    }
}
