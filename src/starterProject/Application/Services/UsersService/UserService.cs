using Application.Features.Users.Rules;
using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using NArchitecture.Core.Persistence.Paging;
using NArchitecture.Core.Security.OAuth.Models;
using System.Linq.Expressions;
using static Domain.Entities.User;

namespace Application.Services.UsersService;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly UserBusinessRules _userBusinessRules;

    public UserService(IUserRepository userRepository, UserBusinessRules userBusinessRules)
    {
        _userRepository = userRepository;
        _userBusinessRules = userBusinessRules;
    }

    public async Task<User?> GetAsync(
        Expression<Func<User, bool>> predicate,
        Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        User? user = await _userRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return user;
    }

    public async Task<IPaginate<User>?> GetListAsync(
        Expression<Func<User, bool>>? predicate = null,
        Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
        Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<User> userList = await _userRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return userList;
    }

    public async Task<User> AddAsync(User user)
    {
        await _userBusinessRules.UserEmailShouldNotExistsWhenInsert(user.Email);

        User addedUser = await _userRepository.AddAsync(user);

        return addedUser;
    }

    public async Task<User> UpdateAsync(User user)
    {
        await _userBusinessRules.UserEmailShouldNotExistsWhenUpdate(user.Id, user.Email);

        User updatedUser = await _userRepository.UpdateAsync(user);

        return updatedUser;
    }

    public async Task<User> DeleteAsync(User user, bool permanent = false)
    {
        user.Status = UserStatus.Deleted;
        User deletedUser = await _userRepository.DeleteAsync(user);

        return deletedUser;
    }
    public async Task<User> CreateOrUpdateExternalUserAsync(ExternalAuthUser externalUser)
    {
        var user = await _userRepository.GetAsync(u => u.Email == externalUser.Email);

        if (user == null)
        {
            user = new User
            {
                Email = externalUser.Email,
                FirstName = externalUser.FirstName,
                LastName = externalUser.LastName,
                Status = UserStatus.Active,
                ExternalAuthProvider = externalUser.Provider
            };
            await _userRepository.AddAsync(user);
        }
        else
        {
            user.FirstName = externalUser.FirstName;
            user.LastName = externalUser.LastName;
            user.ExternalAuthProvider = externalUser.Provider;
            await _userRepository.UpdateAsync(user);
        }

        return user;
    }
}
