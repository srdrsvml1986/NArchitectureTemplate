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

    public UserSessionService(IUserSessionRepository userSessionRepository, UserSessionBusinessRules userSessionBusinessRules)
    {
        _userSessionRepository = userSessionRepository;
        _userSessionBusinessRules = userSessionBusinessRules;

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


 #region Yeni Eklenen Metotlar
 

    /// <summary>
    /// Sistemdeki toplam aktif oturum sayısını getirir
    /// </summary>
    public async Task<int> GetActiveSessionCountAsync()
    {
        var result = await _userSessionRepository.GetListAsync(
            predicate: s => !s.IsRevoked
        );
        return result.Count;
    }

    /// <summary>
    /// Kullanıcının aktif oturum sayısını getirir
    /// </summary>
    public async Task<int> GetMyActiveSessionCountAsync(Guid userId)
    {
        var result = await _userSessionRepository.GetListAsync(
            predicate: s => s.UserId == userId && !s.IsRevoked
        );
        return result.Count;
    }

    /// <summary>
    /// Belirtilen kullanıcının oturumlarını listeler
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
