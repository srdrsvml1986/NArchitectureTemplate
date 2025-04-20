using System.Collections.Immutable;
using Application.Services.Repositories;
using Application.Services.UsersService;
using AutoMapper;
using Domain.DTos;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using MimeKit;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Mailing;
using NArchitecture.Core.Security.EmailAuthenticator;
using NArchitecture.Core.Security.Enums;
using NArchitecture.Core.Security.JWT;
using NArchitecture.Core.Security.OAuth.Models;
using NArchitecture.Core.Security.OtpAuthenticator;

namespace Application.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenHelper<Guid, int, Guid> _tokenHelper;
    private readonly TokenOptions _tokenOptions;
    private readonly IUserSecurityClaimRepository _userOperationClaimRepository;
    private readonly IMapper _mapper;
    private readonly IEmailAuthenticatorHelper _emailAuthenticatorHelper;
    private readonly IEmailAuthenticatorRepository _emailAuthenticatorRepository;
    private readonly IMailService _mailService;
    private readonly IOtpAuthenticatorHelper _otpAuthenticatorHelper;
    private readonly IOtpAuthenticatorRepository _otpAuthenticatorRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly string _appName;

    public AuthService(
        IUserSecurityClaimRepository userOperationClaimRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenHelper<Guid, int, Guid> tokenHelper,
        IConfiguration configuration,
        IMapper mapper,
        IMailService mailService,
        IOtpAuthenticatorRepository otpAuthenticatorRepository,
        IOtpAuthenticatorHelper otpAuthenticatorHelper,
        IEmailAuthenticatorRepository emailAuthenticatorRepository,
        IEmailAuthenticatorHelper emailAuthenticatorHelper,
        IUserRepository userRepository,
        IUserService userService)
    {
        _userOperationClaimRepository = userOperationClaimRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenHelper = tokenHelper;

        const string tokenOptionsConfigurationSection = "TokenOptions";
        _tokenOptions =
            configuration.GetSection(tokenOptionsConfigurationSection).Get<TokenOptions>()
            ?? throw new NullReferenceException($"\"{tokenOptionsConfigurationSection}\" bölümü konfigürasyonda bulunamadı.");
        _appName = configuration.GetValue<string>("AppName")
            ?? throw new NullReferenceException("\"AppName\" bölümü konfigürasyonda bulunamadı.");
        _mapper = mapper;
        _mailService = mailService;
        _otpAuthenticatorRepository = otpAuthenticatorRepository;
        _otpAuthenticatorHelper = otpAuthenticatorHelper;
        _emailAuthenticatorRepository = emailAuthenticatorRepository;
        _emailAuthenticatorHelper = emailAuthenticatorHelper;
        _userRepository = userRepository;
        _userService = userService;
    }

    public async Task<AccessToken> CreateAccessToken(User user)
    {
        IList<SecurityClaim> operationClaims = await _userOperationClaimRepository.GetSecurityClaimsByUserIdAsync(user.Id);
        AccessToken accessToken = _tokenHelper.CreateToken(
            user,
            operationClaims.Select(op => (NArchitecture.Core.Security.Entities.SecurityClaim<int>)op).ToImmutableList()
        );
        return accessToken;
    }

    public async Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken)
    {
        RefreshToken addedRefreshToken = await _refreshTokenRepository.AddAsync(refreshToken);
        return addedRefreshToken;
    }

    public async Task DeleteOldRefreshTokens(Guid userId)
    {
        List<RefreshToken> refreshTokens = await _refreshTokenRepository.GetOldRefreshTokensAsync(
            userId,
            _tokenOptions.RefreshTokenTTL
        );
        await _refreshTokenRepository.DeleteRangeAsync(refreshTokens);
    }

    public async Task<RefreshToken?> GetRefreshTokenByToken(string token)
    {
        RefreshToken? refreshToken = await _refreshTokenRepository.GetAsync(predicate: r => r.Token == token);
        return refreshToken;
    }

    public async Task RevokeRefreshToken(
        RefreshToken refreshToken,
        string ipAddress,
        string? reason = null,
        string? replacedByToken = null
    )
    {
        refreshToken.RevokedDate = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReasonRevoked = reason;
        refreshToken.ReplacedByToken = replacedByToken;
        await _refreshTokenRepository.UpdateAsync(refreshToken);
    }

    public async Task<RefreshToken> RotateRefreshToken(User user, RefreshToken refreshToken, string ipAddress)
    {
        NArchitecture.Core.Security.Entities.RefreshToken<Guid, Guid> newCoreRefreshToken = _tokenHelper.CreateRefreshToken(
            user,
            ipAddress
        );
        RefreshToken newRefreshToken = _mapper.Map<RefreshToken>(newCoreRefreshToken);
        await RevokeRefreshToken(refreshToken, ipAddress, reason: "Yeni token ile değiştirildi", newRefreshToken.Token);
        return newRefreshToken;
    }

    public async Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, string ipAddress, string reason)
    {
        RefreshToken? childToken = await _refreshTokenRepository.GetAsync(predicate: r =>
            r.Token == refreshToken.ReplacedByToken
        );

        if (childToken?.RevokedDate != null && childToken.ExpirationDate <= DateTime.UtcNow)
            await RevokeRefreshToken(childToken, ipAddress, reason);
        else
            await RevokeDescendantRefreshTokens(refreshToken: childToken!, ipAddress, reason);
    }

    public Task<RefreshToken> CreateRefreshToken(User user, string ipAddress)
    {
        NArchitecture.Core.Security.Entities.RefreshToken<Guid, Guid> coreRefreshToken = _tokenHelper.CreateRefreshToken(
            user,
            ipAddress
        );
        RefreshToken refreshToken = _mapper.Map<RefreshToken>(coreRefreshToken);
        return Task.FromResult(refreshToken);
    }

    public async Task<EmailAuthenticator> CreateEmailAuthenticator(User user)
    {
        EmailAuthenticator emailAuthenticator =
            new()
            {
                UserId = user.Id,
                ActivationKey = await _emailAuthenticatorHelper.CreateEmailActivationKey(),
                IsVerified = false
            };
        return emailAuthenticator;
    }

    public async Task<OtpAuthenticator> CreateOtpAuthenticator(User user)
    {
        OtpAuthenticator otpAuthenticator =
            new()
            {
                UserId = user.Id,
                SecretKey = await _otpAuthenticatorHelper.GenerateSecretKey(),
                IsVerified = false
            };
        return otpAuthenticator;
    }

    public async Task<string> ConvertSecretKeyToString(byte[] secretKey)
    {
        string result = await _otpAuthenticatorHelper.ConvertSecretKeyToString(secretKey);
        return result;
    }

    public async Task SendAuthenticatorCode(User user)
    {
        if (user.AuthenticatorType is AuthenticatorType.Email)
            await SendAuthenticatorCodeWithEmail(user);
    }

    public async Task VerifyAuthenticatorCode(User user, string authenticatorCode)
    {
        if (user.AuthenticatorType is AuthenticatorType.Email)
            await VerifyAuthenticatorCodeWithEmail(user, authenticatorCode);
        else if (user.AuthenticatorType is AuthenticatorType.Otp)
            await VerifyAuthenticatorCodeWithOtp(user, authenticatorCode);
    }

    private async Task SendAuthenticatorCodeWithEmail(User user)
    {
        EmailAuthenticator? emailAuthenticator = await _emailAuthenticatorRepository.GetAsync(predicate: e =>
            e.UserId == user.Id
        );
        if (emailAuthenticator is null)
            throw new NotFoundException("E-posta Doğrulayıcı bulunamadı.");
        if (!emailAuthenticator.IsVerified)
            throw new BusinessException("E-posta Doğrulayıcı doğrulanmış olmalıdır.");

        string authenticatorCode = await _emailAuthenticatorHelper.CreateEmailActivationCode();
        emailAuthenticator.ActivationKey = authenticatorCode;
        await _emailAuthenticatorRepository.UpdateAsync(emailAuthenticator);

        var toEmailList = new List<MailboxAddress> { new(name: user.Email, address: user.Email) };

        _mailService.SendMail(
            new Mail
            {
                ToList = toEmailList,
                Subject = $"Doğrulama Kodu - {_appName}",
                TextBody = $"Doğrulama kodunuz: {authenticatorCode}"
            }
        );
    }

    private async Task VerifyAuthenticatorCodeWithEmail(User user, string authenticatorCode)
    {
        EmailAuthenticator? emailAuthenticator = await _emailAuthenticatorRepository.GetAsync(predicate: e =>
            e.UserId == user.Id
        );
        if (emailAuthenticator is null)
            throw new NotFoundException("E-posta Doğrulayıcı bulunamadı.");
        if (emailAuthenticator.ActivationKey != authenticatorCode)
            throw new BusinessException("Doğrulama kodu geçersiz.");
        emailAuthenticator.ActivationKey = null;
        await _emailAuthenticatorRepository.UpdateAsync(emailAuthenticator);
    }

    private async Task VerifyAuthenticatorCodeWithOtp(User user, string authenticatorCode)
    {
        OtpAuthenticator? otpAuthenticator = await _otpAuthenticatorRepository.GetAsync(predicate: e => e.UserId == user.Id);
        if (otpAuthenticator is null)
            throw new NotFoundException("OTP Doğrulayıcı bulunamadı.");
        bool result = await _otpAuthenticatorHelper.VerifyCode(otpAuthenticator.SecretKey, authenticatorCode);
        if (!result)
            throw new BusinessException("Doğrulama kodu geçersiz.");
    }

    // OAuth ile gelen kullanıcı için token oluşturma
    public async Task<TokenDto> CreateTokenForExternalUser(ExternalAuthUser externalUser)
    {
        // Kullanıcıyı bul veya oluştur
        var user = await _userService.CreateOrUpdateExternalUserAsync(externalUser);

        // Access token oluştur
        var accessToken = await CreateAccessToken(user);

        // Refresh token oluştur
        var refreshToken = await CreateRefreshToken(user, externalUser.IpAddress);
        await _refreshTokenRepository.AddAsync(refreshToken);

        return new TokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}