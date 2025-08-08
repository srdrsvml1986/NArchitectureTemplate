using NArchitecture.Core.Persistence.Paging;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.UserSessions;

public interface IUserSessionService
{
    Task<UserSession?> GetAsync(
        Expression<Func<UserSession, bool>> predicate,
        Func<IQueryable<UserSession>, IIncludableQueryable<UserSession, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<UserSession>?> GetListAsync(
        Expression<Func<UserSession, bool>>? predicate = null,
        Func<IQueryable<UserSession>, IOrderedQueryable<UserSession>>? orderBy = null,
        Func<IQueryable<UserSession>, IIncludableQueryable<UserSession, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<UserSession> AddAsync(UserSession userSession);
    Task<UserSession> UpdateAsync(UserSession userSession);
    Task<UserSession> DeleteAsync(UserSession userSession, bool permanent = false);
    Task<int> GetActiveSessionCountAsync();
    Task<IEnumerable<UserSession>> GetUserSessionsAsync(Guid userId);
}
