using Application.Features.UserSessions.Rules;
using Application.Services.Repositories;
using NArchitectureTemplate.Core.Persistence.Paging;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

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
}
