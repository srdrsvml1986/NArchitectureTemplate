using Application.Features.Auth.Constants;
using Application.Services;
using Application.Services.Repositories;
using Domain.Entities;
using Elasticsearch.Net;
using NArchitectureTemplate.Core.Application.Rules;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.Types;
using NArchitectureTemplate.Core.Localization.Abstraction;
using NArchitectureTemplate.Core.Security.Enums;
using NArchitectureTemplate.Core.Security.Hashing;
using Nest;
using Org.BouncyCastle.Asn1.Ocsp;
using static Domain.Entities.User;

namespace Application.Features.Auth.Rules;

public class AuthBusinessRules : BaseBusinessRules
{
    private readonly IUserRepository _userRepository;
    private readonly ILocalizationService _localizationService;
    private readonly AuditService _auditService;

    public AuthBusinessRules(IUserRepository userRepository, ILocalizationService localizationService, AuditService auditService)
    {
        _userRepository = userRepository;
        _localizationService = localizationService;
        _auditService = auditService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, AuthMessages.SectionName);
         
                _auditService.LogAccess(
                "AUTH",
                "LOGIN_FAILED: "+ message,
                "",
                ""
                );
        
        throw new BusinessException(message);
    }

    public async Task EmailAuthenticatorShouldBeExists(EmailAuthenticator? emailAuthenticator)
    {
        if (emailAuthenticator is null)
            await throwBusinessException(AuthMessages.EmailAuthenticatorDontExists);
    }

    public async Task OtpAuthenticatorShouldBeExists(OtpAuthenticator? otpAuthenticator)
    {
        if (otpAuthenticator is null)
            await throwBusinessException(AuthMessages.OtpAuthenticatorDontExists);
    }

    public async Task OtpAuthenticatorThatVerifiedShouldNotBeExists(OtpAuthenticator? otpAuthenticator)
    {
        if (otpAuthenticator is not null && otpAuthenticator.IsVerified)
            await throwBusinessException(AuthMessages.AlreadyVerifiedOtpAuthenticatorIsExists);
    }

    public async Task EmailAuthenticatorActivationKeyShouldBeExists(EmailAuthenticator emailAuthenticator)
    {
        if (emailAuthenticator.ActivationKey is null)
            await throwBusinessException(AuthMessages.EmailActivationKeyDontExists);
    }

    public async Task UserShouldBeExistsWhenSelected(User? user)
    {
        if (user == null)
            await throwBusinessException(AuthMessages.UserDontExists);
    }

    public async Task UserShouldNotBeHaveAuthenticator(User user)
    {
        if (user.AuthenticatorType != AuthenticatorType.None)
            await throwBusinessException(AuthMessages.UserHaveAlreadyAuthenticator);
    }

    public async Task RefreshTokenShouldBeExists(RefreshToken? refreshToken)
    {
        if (refreshToken == null)
            await throwBusinessException(AuthMessages.RefreshDontExists);
    }

    public async Task RefreshTokenShouldBeActive(RefreshToken refreshToken)
    {
        if (refreshToken.RevokedDate != null && DateTime.UtcNow >= refreshToken.ExpirationDate)
            await throwBusinessException(AuthMessages.InvalidRefreshToken);
    }

    public async Task UserEmailShouldBeNotExists(string email)
    {
        bool doesExists = await _userRepository.AnyAsync(predicate: u => u.Email == email);
        if (doesExists)
            await throwBusinessException(AuthMessages.UserMailAlreadyExists);
    }

    public async Task UserPasswordShouldBeMatch(User user, string password)
    {
        if (!HashingHelper.VerifyPasswordHash(password, user!.PasswordHash, user.PasswordSalt))
            await throwBusinessException(AuthMessages.PasswordDontMatch);
    }
    public async Task UserShouldBeActive(User user)
    {
        if (user.Status != UserStatus.Active)
        {
            string errorMessage = user.Status switch
            {
                UserStatus.Unverified => AuthMessages.UserStatusUnverified,
                UserStatus.Inactive => AuthMessages.UserStatusInactive,
                UserStatus.Suspended => AuthMessages.UserStatusSuspended,
                UserStatus.Deleted => AuthMessages.UserStatusDeleted,
                _ => AuthMessages.UserStatusOther
            };
            await throwBusinessException(errorMessage);
        }
    }
}
