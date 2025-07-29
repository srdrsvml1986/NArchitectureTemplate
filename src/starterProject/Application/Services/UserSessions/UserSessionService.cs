using Application.Features.UserSessions.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using Application.Features.Auth.Commands.RevokeToken;
using Application.Services.AuthService;
using MediatR;

namespace Application.Services.UserSessions;

public class UserSessionService : IUserSessionService
{
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly UserSessionBusinessRules _userSessionBusinessRules;
    private readonly INotificationService _notificationService;
    private readonly IAuthService _authService;
    private readonly IMediator _mediator;

    public UserSessionService(IUserSessionRepository userSessionRepository, UserSessionBusinessRules userSessionBusinessRules, INotificationService notificationService, IAuthService authService, IMediator mediator)
    {
        _userSessionRepository = userSessionRepository;
        _userSessionBusinessRules = userSessionBusinessRules;
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
            predicate: s => s.UserId == userId && !s.IsRevoked
        )).Items;
    }

    /// <summary>
    /// Þüpheli oturumlarý tespit eder ve gerekli iþlemleri yapar.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
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
