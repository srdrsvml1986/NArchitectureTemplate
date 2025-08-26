using Application.Features.UserSessions.Rules;
using Application.Services.NotificationServices;
using Application.Services.Repositories;
using Application.Services.UsersService;
using Microsoft.EntityFrameworkCore.Query;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction;
using NArchitectureTemplate.Core.Persistence.Paging;
using System.Linq.Expressions;

namespace Application.Services.UserSessions;

public class UserSessionService : IUserSessionService
{
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly IPushNotificationService _pushNotificationService;
    private readonly IUserService _userService;
    private readonly ILogger _logger;
    private readonly UserSessionBusinessRules _userSessionBusinessRules;

    public UserSessionService(IUserSessionRepository userSessionRepository, UserSessionBusinessRules userSessionBusinessRules, IPushNotificationService pushNotificationService, IUserService userService, ILogger logger)
    {
        _userSessionRepository = userSessionRepository;
        _userSessionBusinessRules = userSessionBusinessRules;
        _pushNotificationService = pushNotificationService;
        _userService = userService;
        _logger = logger;
    }

    public async Task<UserSession?> GetAsync(
        Expression<Func<UserSession, bool>> predicate,
        Func<IQueryable<UserSession>, IIncludableQueryable<UserSession, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        UserSession? userSession = await _userSessionRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return userSession;
    }

    public async Task<IPaginate<UserSession>?> GetListAsync(
        Expression<Func<UserSession, bool>>? predicate = null,
        Func<IQueryable<UserSession>, IOrderedQueryable<UserSession>>? orderBy = null,
        Func<IQueryable<UserSession>, IIncludableQueryable<UserSession, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<UserSession> userSessionList = await _userSessionRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return userSessionList;
    }

    public async Task<UserSession> AddAsync(UserSession userSession, bool enableTracking = true)
    {
        UserSession addedUserSession = await _userSessionRepository.AddAsync(userSession, enableTracking: enableTracking);

        return addedUserSession;
    }

    public async Task<UserSession> UpdateAsync(UserSession userSession, bool enableTracking = true)
    {
        UserSession updatedUserSession = await _userSessionRepository.UpdateAsync(userSession, enableTracking);

        return updatedUserSession;
    }
    public async Task<ICollection<UserSession>> UpdateAllAsync(ICollection<UserSession> userSession, bool enableTracking = true)
    {
        ICollection<UserSession> updatedUserSession = await _userSessionRepository.UpdateRangeAsync(userSession, enableTracking: enableTracking);

        return updatedUserSession;
    }

    public async Task<UserSession> DeleteAsync(UserSession userSession, bool permanent = false)
    {
        UserSession deletedUserSession = await _userSessionRepository.DeleteAsync(userSession);

        return deletedUserSession;
    }


    #region Yeni Eklenen Metotlar


    /// <summary>
    /// Sistemdeki toplam aktif oturum sayýsýný getirir
    /// </summary>
    public async Task<int> GetActiveSessionCountAsync()
    {
        var result = await _userSessionRepository.GetListAsync(
            predicate: s => !s.IsRevoked
        );
        return result.Count;
    }

    /// <summary>
    /// Kullanýcýnýn aktif oturum sayýsýný getirir
    /// </summary>
    public async Task<int> GetMyActiveSessionCountAsync(Guid userId)
    {
        var result = await _userSessionRepository.GetListAsync(
            predicate: s => s.UserId == userId && !s.IsRevoked
        );
        return result.Count;
    }

    /// <summary>
    /// Belirtilen kullanýcýnýn oturumlarýný listeler
    /// </summary>
    public async Task<IEnumerable<UserSession>> GetUserSessionsAsync(Guid userId)
    {
        var result = await _userSessionRepository.GetListAsync(
            predicate: s => s.UserId == userId,
            orderBy: q => q.OrderByDescending(s => s.LoginTime)
        );
        return result.Items;
    }

    #endregion

    private async Task CheckSuspiciousSessionAndNotifyAsync(UserSession session)
    {
        try
        {
            var previousSessions = await _userSessionRepository.GetListAsync(
                predicate: s => s.UserId == session.UserId && s.Id != session.Id,
                orderBy: q => q.OrderByDescending(s => s.LoginTime),
                size: 5
            );

            if (previousSessions.Items.Any())
            {
                var lastSession = previousSessions.Items.First();

                // Farklý IP veya konumdan giriþ tespit edilirse
                if (lastSession.IpAddress != session.IpAddress)
                {
                    // Kullanýcýnýn device token'larýný al
                    var userTokens = await _userService.GetUserDeviceTokensAsync(session.UserId);

                    if (userTokens.Any())
                    {
                        var notification = new PushNotification
                        {
                            Title = "Þüpheli Oturum Tespit Edildi",
                            Body = $"Hesabýnýza {session.IpAddress} IP adresinden yeni bir oturum açýldý. Son oturumunuz {lastSession.IpAddress} IP adresindendi.",
                            DeviceTokens = userTokens,
                            Data = new Dictionary<string, string>
                            {
                                {"eventType", "suspiciousSession"},
                                {"sessionId", session.Id.ToString()},
                                {"ipAddress", session.IpAddress},
                                {"loginTime", session.LoginTime.ToString("O")}
                            },
                            Priority = PushNotificationPriority.High
                        };

                        await _pushNotificationService.SendAsync(notification);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex,ex.Message);
        }
    }
}
