using System.Linq.Expressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using NArchitectureTemplate.Core.Persistence.Paging;
using NArchitectureTemplate.Core.Security.OAuth.Models;

namespace Application.Services.UsersService;

public interface IUserService
{
    Task<User?> GetAsync(
        Expression<Func<User, bool>> predicate,
        Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<IPaginate<User>?> GetListAsync(
        Expression<Func<User, bool>>? predicate = null,
        Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
        Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<User> DeleteAsync(User user, bool permanent = false);
    Task<User> CreateOrUpdateExternalUserAsync(ExternalAuthUser externalUser);
    Task AddDeviceTokenAsync(Guid userId, string token, string deviceType, string deviceName = null);
    Task RemoveDeviceTokenAsync(string token);
    Task<List<string>> GetUserDeviceTokensAsync(Guid userId);
    Task DeactivateDeviceTokenAsync(string token);
    Task AddLoginAttempt(User user);
    Task<bool> UserEmailShouldBeNotExists(string email);
    Task<bool> UserEmailShouldNotExistsWhenUpdate(Guid id, string email);
    Task<bool> UserEmailShouldNotExistsWhenInsert(string email);
    Task<bool> UserIdShouldBeExistsWhenSelected(Guid id);
}
