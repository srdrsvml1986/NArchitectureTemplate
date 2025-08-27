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
    private readonly UserBusinessRules _userBusinessRules;
    private readonly IPushNotificationService _pushNotificationService;
    private readonly IDeviceTokenService _deviceTokenService;


    public UserService(IUserRepository userRepository, UserBusinessRules userBusinessRules, IPushNotificationService pushNotificationService, IDeviceTokenService deviceTokenService)
    {
        _userRepository = userRepository;
        _userBusinessRules = userBusinessRules;
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
        //try
        //{
        //    var adminUsers = await _userRepository.GetListAsync(
        //        predicate: u => u.UserRoles.Any(ur => ur.Role.Name == "Admin"),
        //        include: u => u.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
        //    );

        //    var adminTokens = adminUsers.Items
        //        .Where(u => u.DeviceTokens != null && u.DeviceTokens.Any())
        //        .SelectMany(u => u.DeviceTokens)
        //        .ToList();

        //    if (adminTokens.Any())
        //    {
        //        var notification = new PushNotification
        //        {
        //            Title = "Yeni Kullanıcı Kaydı",
        //            Body = $"{newUser.Email} email adresli yeni bir kullanıcı kayıt oldu.",
        //            DeviceTokens = adminTokens,
        //            Data = new Dictionary<string, string>
        //            {
        //                {"eventType", "userRegistered"},
        //                {"userId", newUser.Id.ToString()},
        //                {"userEmail", newUser.Email}
        //            },
        //            Priority = PushNotificationPriority.Normal
        //        };

        //        await _pushNotificationService.SendAsync(notification);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    // Bildirim hatası ana işlemi etkilememeli
        //    // Loglama yapılabilir
        //}
    }
}
