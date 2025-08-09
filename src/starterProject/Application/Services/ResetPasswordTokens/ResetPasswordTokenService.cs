using Application.Services.Repositories;
using NArchitectureTemplate.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.PasswordResetTokens;

public class PasswordResetTokenService : IPasswordResetTokenService
{
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;

    public PasswordResetTokenService(IPasswordResetTokenRepository passwordResetTokenRepository)
    {
        _passwordResetTokenRepository = passwordResetTokenRepository;
    }

    public async Task<ResetPasswordToken?> GetAsync(
        Expression<Func<ResetPasswordToken, bool>> predicate,
        Func<IQueryable<ResetPasswordToken>, IIncludableQueryable<ResetPasswordToken, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        ResetPasswordToken? passwordResetToken = await _passwordResetTokenRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return passwordResetToken;
    }

    public async Task<IPaginate<ResetPasswordToken>?> GetListAsync(
        Expression<Func<ResetPasswordToken, bool>>? predicate = null,
        Func<IQueryable<ResetPasswordToken>, IOrderedQueryable<ResetPasswordToken>>? orderBy = null,
        Func<IQueryable<ResetPasswordToken>, IIncludableQueryable<ResetPasswordToken, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<ResetPasswordToken> passwordResetTokenList = await _passwordResetTokenRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return passwordResetTokenList;
    }

    public async Task<ResetPasswordToken> AddAsync(ResetPasswordToken passwordResetToken)
    {
        ResetPasswordToken addedPasswordResetToken = await _passwordResetTokenRepository.AddAsync(passwordResetToken);

        return addedPasswordResetToken;
    }

    public async Task<ResetPasswordToken> UpdateAsync(ResetPasswordToken passwordResetToken)
    {
        ResetPasswordToken updatedPasswordResetToken = await _passwordResetTokenRepository.UpdateAsync(passwordResetToken);

        return updatedPasswordResetToken;
    }

    public async Task<ResetPasswordToken> DeleteAsync(ResetPasswordToken passwordResetToken, bool permanent = false)
    {
        ResetPasswordToken deletedPasswordResetToken = await _passwordResetTokenRepository.DeleteAsync(passwordResetToken);

        return deletedPasswordResetToken;
    }
}
