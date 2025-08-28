using Application.Features.Users.Rules;
using Application.Services.DeviceTokens;
using Application.Services.NotificationServices;
using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NArchitectureTemplate.Core.Notification.Services;
using NArchitectureTemplate.Core.Persistence.Paging;
using NArchitectureTemplate.Core.Security.OAuth.Models;
using System.Linq.Expressions;
using static Domain.Entities.User;

namespace Application.Services.UsersService;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPushNotificationService _pushNotificationService;
    private readonly IDeviceTokenService _deviceTokenService;


    public UserService(IUserRepository userRepository, IPushNotificationService pushNotificationService, IDeviceTokenService deviceTokenService)
    {
        _userRepository = userRepository;
        _pushNotificationService = pushNotificationService;
        _deviceTokenService = deviceTokenService;
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
        bool doesExists = await UserEmailShouldNotExistsWhenInsert(user.Email);
        if (doesExists)
            throw new Exception("User email already exists.");
        user.CreatedDate = DateTime.UtcNow;
        User addedUser = await _userRepository.AddAsync(user);

        await SendNewUserNotificationToAdmins(user);
        return addedUser;
    }

    public async Task<User> UpdateAsync(User user)
    {
        bool doesExists = await UserEmailShouldNotExistsWhenUpdate(user.Id, user.Email);
        if (doesExists)
            throw new Exception("User email already exists.");

        user.UpdatedDate = DateTime.UtcNow;
        User updatedUser = await _userRepository.UpdateAsync(user);
        return updatedUser;
    }

    public async Task<User> DeleteAsync(User user, bool permanent = false)
    {
        user.Status = UserStatus.Deleted;
        user.DeletedDate = DateTime.UtcNow;
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
                ExternalAuthProvider = externalUser.Provider,
                lastActivityDate = DateTime.UtcNow,
                CreatedDate= DateTime.UtcNow
            };
            await _userRepository.AddAsync(user);
            await SendNewUserNotificationToAdmins(user);
        }
        else
        {
            user.FirstName = externalUser.FirstName;
            user.LastName = externalUser.LastName;
            user.ExternalAuthProvider = externalUser.Provider;
            user.lastActivityDate = DateTime.UtcNow;
            user.UpdatedDate = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }

        return user;
    }

    public async Task AddDeviceTokenAsync(Guid userId, string token, string deviceType, string deviceName = null)
    {
        var deviceToken = new DeviceToken
        {
            UserId = userId,
            Token = token,
            DeviceType = deviceType,
            DeviceName = deviceName,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _deviceTokenService.AddAsync(deviceToken);
    }

    public async Task RemoveDeviceTokenAsync(string token)
    {
        await _deviceTokenService.RemoveWithTokenAsync(token);
    }

    public async Task<List<string>> GetUserDeviceTokensAsync(Guid userId)
    {
        return await _deviceTokenService.GetUserDeviceTokensAsync(userId);
    }

    public async Task DeactivateDeviceTokenAsync(string token)
    {
        await _deviceTokenService.DeactivateTokenAsync(token);
    }
    private async Task SendNewUserNotificationToAdmins(User newUser)
    {
        try
        {
            var adminUsers = await _userRepository.GetListAsync(
                predicate: u => u.UserRoles.Any(ur => ur.Role.Name == "Admin"),
                include: u => u.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            );

            var adminTokens = adminUsers.Items
                .Where(u => u.DeviceTokens != null && u.DeviceTokens.Any())
                .SelectMany(u => u.DeviceTokens).Select(d=> d.Token)
                .ToList(); 

            if (adminTokens.Any())
            {
                var notification = new PushNotification
                {
                    Title = "Yeni Kullanıcı Kaydı",
                    Body = $"{newUser.Email} email adresli yeni bir kullanıcı kayıt oldu.",
                    DeviceTokens = adminTokens,
                    Data = new Dictionary<string, string>
                    {
                        {"eventType", "userRegistered"},
                        {"userId", newUser.Id.ToString()},
                        {"userEmail", newUser.Email}
                    },
                    Priority = PushNotificationPriority.Normal
                };

                await _pushNotificationService.SendAsync(notification);
            }
        }
        catch (Exception ex)
        {
            // Bildirim hatası ana işlemi etkilememeli
            // Loglama yapılabilir
        }
    }

    public Task AddLoginAttempt(User user)
    {
        user.lastActivityDate = DateTime.UtcNow;
        return _userRepository.UpdateAsync(user);
    }

    public Task<bool> UserEmailShouldBeNotExists(string email) => _userRepository.AnyAsync(predicate: u => u.Email == email);
    public Task<bool> UserEmailShouldNotExistsWhenInsert(string email) => _userRepository.AnyAsync(predicate: u => u.Email == email);
    public Task<bool> UserIdShouldBeExistsWhenSelected(Guid id) => _userRepository.AnyAsync(predicate: u => u.Id == id);
    public Task<bool> UserEmailShouldNotExistsWhenUpdate(Guid id, string email) => _userRepository.AnyAsync(predicate: u => u.Id != id && u.Email == email);
}
