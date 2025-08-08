using Application.Features.UserSessions.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using Application.Features.Auth.Commands.RevokeToken;
using Application.Services.AuthService;
using MediatR;
using Application.Features.Users.Rules;

namespace Application.Services.UserSessions;

public class UserSessionService : IUserSessionService
{
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly UserSessionBusinessRules _userSessionBusinessRules;
    private readonly INotificationService _notificationService;
    private readonly IAuthService _authService;
    private readonly IMediator _mediator;
    private IUserRepository _userRepository;
    private UserBusinessRules _userBusinessRules;

    public UserSessionService(IUserSessionRepository userSessionRepository, UserSessionBusinessRules userSessionBusinessRules, INotificationService notificationService, IAuthService authService, IMediator mediator)
    {
        _userSessionRepository = userSessionRepository;
        _userSessionBusinessRules = userSessionBusinessRules;
        _notificationService = notificationService;
        _authService = authService;
        _mediator = mediator;
    }

    public UserSessionService(IUserRepository userRepository, UserBusinessRules userBusinessRules)
    {
        _userRepository = userRepository;
        _userBusinessRules = userBusinessRules;
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

    public async Task<UserSession> AddAsync(UserSession userSession)
    {
        UserSession addedUserSession = await _userSessionRepository.AddAsync(userSession);

        return addedUserSession;
    }

    public async Task<UserSession> UpdateAsync(UserSession userSession)
    {
        UserSession updatedUserSession = await _userSessionRepository.UpdateAsync(userSession);

        return updatedUserSession;
    }

    public async Task<UserSession> DeleteAsync(UserSession userSession, bool permanent = false)
    {
        UserSession deletedUserSession = await _userSessionRepository.DeleteAsync(userSession);

        return deletedUserSession;
    }
    public async Task<IEnumerable<UserSession>> GetActiveSessionsAsync(Guid userId)
    {
        return (await _userSessionRepository.GetListAsync(
            predicate: s => s.UserId == userId && !s.IsRevoked, enableTracking: false, orderBy: q => q.OrderByDescending(s => s.LoginTime), cancellationToken: default
        )).Items;
    }

    /// <summary>
    /// ��pheli oturumlar� tespit eder ve gerekli i�lemleri yapar.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task FlagAndHandleSuspiciousSessionsAsync(Guid userId)
    {
        var sessions = (await GetActiveSessionsAsync(userId)).ToList();
        var now = DateTime.UtcNow;
        foreach (var session in sessions)
        {
            // Rule1: Ayn� anda >3 oturum
            if (sessions.Count > 3)
                session.IsSuspicious = true;

            // Rule2: Farkl� lokasyon
            var recent = sessions.OrderByDescending(s => s.LoginTime).FirstOrDefault();
            if (recent != null && session != recent && session.LocationInfo != recent.LocationInfo)
                session.IsSuspicious = true;

            // Rule3: K�sa s�rede �ok say�da token yenileme
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
    #region Yeni Eklenen Metotlar

    /// <summary>
    /// Mevcut oturum d���ndaki t�m oturumlar� sonland�r�r
    /// </summary>
    public async Task RevokeAllOtherSessionsAsync(Guid userId, Guid currentSessionId)
    {
        var sessions = (await GetActiveSessionsAsync(userId))
            .Where(s => s.Id != currentSessionId);

        foreach (var session in sessions)
        {
            var token = await _authService.GetRefreshTokenBySessionAsync(session.Id);
            await _mediator.Send(new RevokeTokenCommand(token, session.IpAddress));

            session.IsRevoked = true;
            await _userSessionRepository.UpdateAsync(session);
        }
    }

    /// <summary>
    /// Kullan�c�n�n kendi oturumunu sonland�r�r
    /// </summary>
    public async Task RevokeMySessionAsync(Guid sessionId, string ipAddress)
    {
        var session = await _userSessionRepository.GetAsync(s => s.Id == sessionId);
        await _userSessionBusinessRules.UserSessionShouldExistWhenSelected(session);

        var token = await _authService.GetRefreshTokenBySessionAsync(sessionId);
        await _mediator.Send(new RevokeTokenCommand(token, ipAddress));

        session!.IsRevoked = true;
        await _userSessionRepository.UpdateAsync(session);
    }

    /// <summary>
    /// Belirtilen kullan�c� oturumunu sonland�r�r
    /// </summary>
    public async Task RevokeUserSessionAsync(Guid sessionId)
    {
        var session = await _userSessionRepository.GetAsync(s => s.Id == sessionId);
        await _userSessionBusinessRules.UserSessionShouldExistWhenSelected(session);

        var token = await _authService.GetRefreshTokenBySessionAsync(sessionId);
        await _mediator.Send(new RevokeTokenCommand(token, session!.IpAddress));

        session.IsRevoked = true;
        await _userSessionRepository.UpdateAsync(session);
    }

    /// <summary>
    /// Sistemdeki toplam aktif oturum say�s�n� getirir
    /// </summary>
    public async Task<int> GetActiveSessionCountAsync()
    {
        var result = await _userSessionRepository.GetListAsync(
            predicate: s => !s.IsRevoked
        );
        return result.Count;
    }

    /// <summary>
    /// Kullan�c�n�n aktif oturum say�s�n� getirir
    /// </summary>
    public async Task<int> GetMyActiveSessionCountAsync(Guid userId)
    {
        var result = await _userSessionRepository.GetListAsync(
            predicate: s => s.UserId == userId && !s.IsRevoked
        );
        return result.Count;
    }

    /// <summary>
    /// Kullan�c�n�n kendi oturumlar�n� listeler
    /// </summary>
    public async Task<IEnumerable<UserSession>> GetMySessionsAsync(Guid userId)
    {
        var result = await _userSessionRepository.GetListAsync(
            predicate: s => s.UserId == userId,
            orderBy: q => q.OrderByDescending(s => s.LoginTime)
        );
        return result.Items;
    }

    /// <summary>
    /// Belirtilen kullan�c�n�n oturumlar�n� listeler
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
}
