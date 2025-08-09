using Application.Features.UserGroups.Rules;
using Application.Services.Repositories;
using NArchitectureTemplate.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.UserGroups;

public class UserGroupService : IUserGroupService
{
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly UserGroupBusinessRules _userGroupBusinessRules;

    public UserGroupService(IUserGroupRepository userGroupRepository, UserGroupBusinessRules userGroupBusinessRules)
    {
        _userGroupRepository = userGroupRepository;
        _userGroupBusinessRules = userGroupBusinessRules;
    }

    public async Task<UserGroup?> GetAsync(
        Expression<Func<UserGroup, bool>> predicate,
        Func<IQueryable<UserGroup>, IIncludableQueryable<UserGroup, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        UserGroup? userGroup = await _userGroupRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return userGroup;
    }

    public async Task<IPaginate<UserGroup>?> GetListAsync(
        Expression<Func<UserGroup, bool>>? predicate = null,
        Func<IQueryable<UserGroup>, IOrderedQueryable<UserGroup>>? orderBy = null,
        Func<IQueryable<UserGroup>, IIncludableQueryable<UserGroup, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<UserGroup> userGroupList = await _userGroupRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return userGroupList;
    }

    public async Task<UserGroup> AddAsync(UserGroup userGroup)
    {
        UserGroup addedUserGroup = await _userGroupRepository.AddAsync(userGroup);

        return addedUserGroup;
    }

    public async Task<UserGroup> UpdateAsync(UserGroup userGroup)
    {
        UserGroup updatedUserGroup = await _userGroupRepository.UpdateAsync(userGroup);

        return updatedUserGroup;
    }

    public async Task<UserGroup> DeleteAsync(UserGroup userGroup, bool permanent = false)
    {
        UserGroup deletedUserGroup = await _userGroupRepository.DeleteAsync(userGroup);

        return deletedUserGroup;
    }
}
