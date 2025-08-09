using NArchitectureTemplate.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.PasswordResetTokens;

public interface IPasswordResetTokenService
{
    Task<ResetPasswordToken?> GetAsync(
        Expression<Func<ResetPasswordToken, bool>> predicate,
        Func<IQueryable<ResetPasswordToken>, IIncludableQueryable<ResetPasswordToken, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<ResetPasswordToken>?> GetListAsync(
        Expression<Func<ResetPasswordToken, bool>>? predicate = null,
        Func<IQueryable<ResetPasswordToken>, IOrderedQueryable<ResetPasswordToken>>? orderBy = null,
        Func<IQueryable<ResetPasswordToken>, IIncludableQueryable<ResetPasswordToken, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<ResetPasswordToken> AddAsync(ResetPasswordToken passwordResetToken);
    Task<ResetPasswordToken> UpdateAsync(ResetPasswordToken passwordResetToken);
    Task<ResetPasswordToken> DeleteAsync(ResetPasswordToken passwordResetToken, bool permanent = false);
}
