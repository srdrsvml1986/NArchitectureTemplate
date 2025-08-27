using System.Collections.Immutable;
using Application.Services.Repositories;
using Application.Services.UserSessions;
using Application.Services.UsersService;
using AutoMapper;
using Domain.DTos;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using MimeKit;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.Types;
using NArchitectureTemplate.Core.Mailing;
using NArchitectureTemplate.Core.Security.EmailAuthenticator;
using NArchitectureTemplate.Core.Security.JWT;
using NArchitectureTemplate.Core.Security.OAuth.Models;
using NArchitectureTemplate.Core.Security.OtpAuthenticator;
using NArchitectureTemplate.Core.Security.Enums;
using Application.Services.NotificationServices;
using NArchitectureTemplate.Core.Notification.Services;

namespace Application.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenHelper<Guid, int, int, int, Guid> _tokenHelper;
    private readonly TokenOptions _tokenOptions;
    private readonly IUserOperationClaimRepository _userOperationClaimRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IMapper _mapper;
    private readonly IEmailAuthenticatorHelper _emailAuthenticatorHelper;
    private readonly IEmailAuthenticatorRepository _emailAuthenticatorRepository;
    private readonly IMailService _mailService;
    private readonly IOtpAuthenticatorHelper _otpAuthenticatorHelper;
    private readonly IOtpAuthenticatorRepository _otpAuthenticatorRepository;
    private readonly IUserService _userService;
    private readonly IUserSessionService _userSessionService;
    private readonly INotificationService _notificationService;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly string _appName;

    public AuthService(
        IUserOperationClaimRepository userOperationClaimRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenHelper<Guid, int, int, int, Guid> tokenHelper,
        IConfiguration configuration,
        IMapper mapper,
        IMailService mailService,
        IOtpAuthenticatorRepository otpAuthenticatorRepository,
        IOtpAuthenticatorHelper otpAuthenticatorHelper,
        IEmailAuthenticatorRepository emailAuthenticatorRepository,
        IEmailAuthenticatorHelper emailAuthenticatorHelper,
        IUserRepository userRepository,
        IUserService userService,
        IUserSessionService userSessionService,
        INotificationService notificationService,
        IUserRoleRepository userRoleRepository,
        IUserGroupRepository userGroupRepository)
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
        _userService = userService;
        _userSessionService = userSessionService;
        _notificationService = notificationService;
        _userRoleRepository = userRoleRepository;
        _userGroupRepository = userGroupRepository;
    }

    public async Task<AccessToken> CreateAccessToken(User user)
    {
        IList<OperationClaim> operationClaims = await _userOperationClaimRepository.GetSecurityClaimsByUserIdAsync(user.Id);
        IList<Role> roles = await _userRoleRepository.GetSecurityRolesByUserIdAsync(user.Id);
        IList<Group> groups = (await _userGroupRepository.GetSecurityGroupsByUserIdAsync(user.Id));

        AccessToken accessToken = _tokenHelper.CreateToken(
            user,
            operationClaims.Select(op => (NArchitectureTemplate.Core.Security.Entities.OperationClaim<int>)op).ToImmutableList(),
            roles.Select(role => (NArchitectureTemplate.Core.Security.Entities.Role<int>)role).ToImmutableList(),
            groups.Select(group => (NArchitectureTemplate.Core.Security.Entities.Group<int>)group).ToImmutableList()
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
        NArchitectureTemplate.Core.Security.Entities.RefreshToken<Guid, Guid> newCoreRefreshToken = _tokenHelper.CreateRefreshToken(
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

    public async Task<RefreshToken> CreateRefreshToken(User user, string ipAddress, string userAgent)
    {
        var coreRefreshToken = _tokenHelper.CreateRefreshToken(user, ipAddress);

        // Yeni ve temiz bir UserSession nesnesi oluştur
        var userSession = new UserSession
        {
            IpAddress = ipAddress,
            UserAgent = userAgent,
            UserId = user.Id
        };

        // UserSession'ı veritabanına ekleyip ID'sini al
        var session = await _userSessionService.AddAsync(userSession, enableTracking: false);

        // RefreshToken'ı manuel oluştur (mapper kullanmadan)
        var refreshToken = new RefreshToken
        {
            UserId = coreRefreshToken.UserId,
            Token = coreRefreshToken.Token,
            ExpirationDate = coreRefreshToken.ExpirationDate,
            CreatedByIp = coreRefreshToken.CreatedByIp,
            CreatedDate = coreRefreshToken.CreatedDate,
            UserSessionId = session.Id // Foreign key ilişkisi
        };

        return refreshToken;
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
    /// <summary>
    /// EnableEmailAuthenticator aktif olan Kullanıcının doğrulayıcı kodunu göndermek için kullanılır. 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
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
    public async Task<TokenDto> CreateTokenForExternalUser(ExternalAuthUser externalUser, string ipAddress, string userAgent)
    {
        // Kullanıcıyı bul veya oluştur
        var user = await _userService.CreateOrUpdateExternalUserAsync(externalUser);

        // Access token oluştur
        var accessToken = await CreateAccessToken(user);

        // Refresh token oluştur
        var refreshToken = await CreateRefreshToken(user, externalUser.IpAddress, userAgent);
        await _refreshTokenRepository.AddAsync(refreshToken);

        return new TokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
    public async Task<int> GetRefreshCountAsync(Guid userId, TimeSpan period)
    {
        var since = DateTime.UtcNow - period;
        return await _refreshTokenRepository.CountAsync(rt => rt.UserId == userId && rt.CreatedDate >= since);
    }

    public async Task<string> GetRefreshTokenBySessionAsync(Guid userId)
    {
        // ÖNCE: !rt.IsRevoked kullanılıyor (hata)
        // SONRA: RevokedDate null kontrolü ile değiştir
        var token = await _refreshTokenRepository.GetAsync(rt =>
            rt.UserId == userId &&
            rt.RevokedDate == null // İptal edilmemiş tokenlar
        );
        return token?.Token ?? string.Empty;
    }
    public async Task<IEnumerable<UserSession>> GetActiveSessionsAsync(Guid userId)
    {
        return (await _userSessionService.GetListAsync(
            predicate: s => s.UserId == userId && !s.IsRevoked, 
            enableTracking: false, 
            orderBy: q => q.OrderByDescending(s => s.LoginTime), 
            cancellationToken: default
        ))!.Items;
    }

    /// <summary>
    /// Şüpheli oturumları tespit eder ve gerekli işlemleri yapar.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task FlagAndHandleSuspiciousSessionsAsync(Guid userId)
    {
        var sessions = (await GetActiveSessionsAsync(userId)).ToList();
        var now = DateTime.UtcNow;
        ICollection<UserSession> suspiciousSessions = new List<UserSession>();
        foreach (var session in sessions)
        {
            // Rule1: Aynı anda >3 oturum
            if (sessions.Count > 3)
                session.IsSuspicious = true;

            // Rule2: Farklı lokasyon
            var recent = sessions.OrderByDescending(s => s.LoginTime).FirstOrDefault();
            if (recent != null && session != recent && session.LocationInfo != recent.LocationInfo)
                session.IsSuspicious = true;

            // Rule3: Kısa sürede çok sayıda token yenileme
            var refreshCount = await GetRefreshCountAsync(session.Id, TimeSpan.FromMinutes(5));
            if (refreshCount > 5)
                session.IsSuspicious = true;

            if (session.IsSuspicious)
            {
                try
                {
                    await _notificationService.NotifySuspiciousSessionAsync(session);
                    var token = await GetRefreshTokenBySessionAsync(session.Id);
                    RefreshToken? refreshToken = await GetRefreshTokenByToken(token);
                    if (refreshToken is not null)
                        await RevokeRefreshToken(refreshToken!, session!.IpAddress, reason: "Token kaldırıldı");
                }
                catch (Exception)
                {


                }
                session.IsRevoked = true;
            }
            session.RefreshTokens = null; // Refresh tokenları temizle
            session.User = null; // Kullanıcı bilgilerini temizle
            suspiciousSessions.Add(session);
        }
        // Güncellenmiş oturumları veritabanına kaydet
        await _userSessionService.UpdateAllAsync(suspiciousSessions, false);
    }


    /// <summary>
    /// Mevcut oturum dışındaki tüm oturumları sonlandırır
    /// </summary>
    public async Task RevokeAllOtherSessionsAsync(Guid userId, Guid currentSessionId)
    {
        var sessions = (await GetActiveSessionsAsync(userId))
            .Where(s => s.Id != currentSessionId);

        foreach (var session in sessions)
        {
            var token = await GetRefreshTokenBySessionAsync(session.Id);
            RefreshToken? refreshToken = await GetRefreshTokenByToken(token);
            if (refreshToken is not null)
                await RevokeRefreshToken(refreshToken!, session!.IpAddress, reason: "Token kaldırıldı");

            session.IsRevoked = true;
            await _userSessionService.UpdateAsync(session);
        }
    }


    /// <summary>
    /// Belirtilen kullanıcı oturumunu sonlandırır
    /// </summary>
    public async Task RevokeUserSessionAsync(Guid sessionId)
    {
        var session = await _userSessionService.GetAsync(s => s.Id == sessionId);
        if (session is not null)
        {
            var token = await GetRefreshTokenBySessionAsync(sessionId);

            Domain.Entities.RefreshToken? refreshToken = await GetRefreshTokenByToken(token);
            if (refreshToken is not null)
                await RevokeRefreshToken(refreshToken!, session!.IpAddress, reason: "Token kaldırıldı");

            session.IsRevoked = true;
            await _userSessionService.UpdateAsync(session);
        }

    }
}