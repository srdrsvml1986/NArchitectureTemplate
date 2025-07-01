using Application.Features.UserSessions.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using Application.Services.AuthService;
using MediatR;
using Application.Features.Auth.Commands.RevokeToken;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Application.Services.UserSessions;

// ILocationService.cs - GeoIP bilgisi almak için servis interface
//public interface ILocationService
//{
//    Task<string> GetLocationAsync(string ipAddress);
//}
//public class LocationService : ILocationService
//{
//    private readonly IGeoIpClient _geoIpClient; // Third-party GeoIP client

//    public LocationService(IGeoIpClient geoIpClient)
//    {
//        _geoIpClient = geoIpClient;
//    }

//    public async Task<string> GetLocationAsync(string ipAddress)
//    {
//        // Örnek: GeoIP API çaðrýsý
//        var info = await _geoIpClient.LookupAsync(ipAddress);
//        return $"{info.City}, {info.Country}";
//    }
//}
// INotificationService.cs - Kullanýcý bildirim servisi
public interface INotificationService
{
    Task NotifySuspiciousSessionAsync(UserSession session);
}
public class NotificationService : INotificationService
{
    private readonly IEmailSender _emailSender;
    private readonly IUserRepository _userRepository;

    public NotificationService(IEmailSender emailSender, IUserRepository userRepository)
    {
        _emailSender = emailSender;
        _userRepository = userRepository;
    }

    public async Task NotifySuspiciousSessionAsync(UserSession session)
    {
        // Kullanýcý bilgilerini al
        var user = await _userRepository.GetAsync(u => u.Id == session.UserId);
        if (user == null) return;

        // E-posta gönderimi
        var subject = "Þüpheli Oturum Bildirimi";
        var body = $"Hesabýnýzdan þüpheli bir oturum algýlandý:\n" +
            $"IP: {session.IpAddress}\n" +
            $"Lokasyon: {session.LocationInfo}\n" +
            $"Zaman: {session.LoginTime}";
        await _emailSender.SendEmailAsync(user.Email, subject, body);
    }
}
// IUserSessionService.cs - Oturum yönetim servisi interface'i


// UserSessionService.cs - Oturum yönetim servisi implementasyonu
public class UserSessionService : IUserSessionService
{
    private readonly IUserSessionRepository _userSessionRepository;
    //private readonly ILocationService _locationService;
    private readonly INotificationService _notificationService;
    private readonly IAuthService _authService;
    private readonly IMediator _mediator;

    public UserSessionService(
        IUserSessionRepository sessionRepository,
        //ILocationService locationService,
        INotificationService notificationService,
        IAuthService authService,
        IMediator mediator)
    {
        _userSessionRepository = sessionRepository;
        //_locationService = locationService;
        _notificationService = notificationService;
        _authService = authService;
        _mediator = mediator;
    }
    public async Task<UserSession?> GetAsync(
        Expression<Func<UserSession, bool>> predicate,
        Func<IQueryable<UserSession>, IIncludableQueryable<UserSession, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
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
        bool enableTracking = true,
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

    public async Task CreateSessionAsync(Guid userId, string ipAddress, string userAgent)
    {
        //var location = await _locationService.GetLocationAsync(ipAddress);
        var session = new UserSession
        {
            UserId = userId,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            LoginTime = DateTime.UtcNow,
            LocationInfo = ""
        };
        await _userSessionRepository.AddAsync(session);
    }

    public async Task<IEnumerable<UserSession>> GetActiveSessionsAsync(Guid userId)
    {
        return (await _userSessionRepository.GetListAsync(
            predicate: s => s.UserId == userId && !s.IsRevoked
        )).Items;
    }

    public async Task FlagAndHandleSuspiciousSessionsAsync(Guid userId)
    {
        var sessions = (await GetActiveSessionsAsync(userId)).ToList();
        var now = DateTime.UtcNow;
        foreach (var session in sessions)
        {
            // Rule1: Ayný anda >3 oturum
            if (sessions.Count > 3)
                session.IsSuspicious = true;

            // Rule2: Farklý lokasyon
            var recent = sessions.OrderByDescending(s => s.LoginTime).FirstOrDefault();
            if (recent != null && session != recent && session.LocationInfo != recent.LocationInfo)
                session.IsSuspicious = true;

            // Rule3: Kýsa sürede çok sayýda token yenileme
            var refreshCount = await _authService.GetRefreshCountAsync(session.Id, TimeSpan.FromMinutes(5));
            if (refreshCount > 5)
                session.IsSuspicious = true;

            if (session.IsSuspicious)
            {
                await _notificationService.NotifySuspiciousSessionAsync(session);
                var token = await _authService.GetRefreshTokenBySessionAsync(session.Id);
                await _mediator.Send(new RevokeTokenCommand(token, session.IpAddress));
                session.IsRevoked = true;
            }
            await _userSessionRepository.UpdateAsync(session);
        }
    }
}

//public class UserSessionManager : IUserSessionService
//{
//    private readonly IUserSessionRepository _userSessionRepository;
//    private readonly UserSessionBusinessRules _userSessionBusinessRules;

//    public UserSessionManager(IUserSessionRepository userSessionRepository, UserSessionBusinessRules userSessionBusinessRules)
//    {
//        _userSessionRepository = userSessionRepository;
//        _userSessionBusinessRules = userSessionBusinessRules;
//    }

//    public async Task<UserSession?> GetAsync(
//        Expression<Func<UserSession, bool>> predicate,
//        Func<IQueryable<UserSession>, IIncludableQueryable<UserSession, object>>? include = null,
//        bool withDeleted = false,
//        bool enableTracking = true,
//        CancellationToken cancellationToken = default
//    )
//    {
//        UserSession? userSession = await _userSessionRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
//        return userSession;
//    }

//    public async Task<IPaginate<UserSession>?> GetListAsync(
//        Expression<Func<UserSession, bool>>? predicate = null,
//        Func<IQueryable<UserSession>, IOrderedQueryable<UserSession>>? orderBy = null,
//        Func<IQueryable<UserSession>, IIncludableQueryable<UserSession, object>>? include = null,
//        int index = 0,
//        int size = 10,
//        bool withDeleted = false,
//        bool enableTracking = true,
//        CancellationToken cancellationToken = default
//    )
//    {
//        IPaginate<UserSession> userSessionList = await _userSessionRepository.GetListAsync(
//            predicate,
//            orderBy,
//            include,
//            index,
//            size,
//            withDeleted,
//            enableTracking,
//            cancellationToken
//        );
//        return userSessionList;
//    }

//    public async Task<UserSession> AddAsync(UserSession userSession)
//    {
//        UserSession addedUserSession = await _userSessionRepository.AddAsync(userSession);

//        return addedUserSession;
//    }

//    public async Task<UserSession> UpdateAsync(UserSession userSession)
//    {
//        UserSession updatedUserSession = await _userSessionRepository.UpdateAsync(userSession);

//        return updatedUserSession;
//    }

//    public async Task<UserSession> DeleteAsync(UserSession userSession, bool permanent = false)
//    {
//        UserSession deletedUserSession = await _userSessionRepository.DeleteAsync(userSession);

//        return deletedUserSession;
//    }
//}
