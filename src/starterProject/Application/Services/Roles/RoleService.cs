using Application.Features.Roles.Rules;
using Application.Services.Repositories;
using NArchitectureTemplate.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.Roles;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly RoleBusinessRules _roleBusinessRules;

    public RoleService(IRoleRepository roleRepository, RoleBusinessRules roleBusinessRules)
    {
        _roleRepository = roleRepository;
        _roleBusinessRules = roleBusinessRules;
    }

    public async Task<Role?> GetAsync(
        Expression<Func<Role, bool>> predicate,
        Func<IQueryable<Role>, IIncludableQueryable<Role, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        Role? role = await _roleRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return role;
    }

    public async Task<IPaginate<Role>?> GetListAsync(
        Expression<Func<Role, bool>>? predicate = null,
        Func<IQueryable<Role>, IOrderedQueryable<Role>>? orderBy = null,
        Func<IQueryable<Role>, IIncludableQueryable<Role, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<Role> roleList = await _roleRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return roleList;
    }

    public async Task<Role> AddAsync(Role role)
    {
        Role addedRole = await _roleRepository.AddAsync(role);

        return addedRole;
    }

    public async Task<Role> UpdateAsync(Role role)
    {
        Role updatedRole = await _roleRepository.UpdateAsync(role);

        return updatedRole;
    }

    public async Task<Role> DeleteAsync(Role role, bool permanent = false)
    {
        Role deletedRole = await _roleRepository.DeleteAsync(role);

        return deletedRole;
    }
}
