using Application.Features.Auth.Commands.DisableEmailAuthenticator;
using Application.Features.Auth.Commands.DisableOtpAuthenticator;
using Application.Features.Auth.Commands.EnableEmailAuthenticator;
using Application.Features.Auth.Commands.EnableOtpAuthenticator;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Commands.RevokeToken;
using Application.Features.Auth.Commands.VerifyEmailAuthenticator;
using Application.Features.Auth.Commands.VerifyOtpAuthenticator;
using Application.Services.AuthService;
using Application.Services.UserSessions;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NArchitecture.Core.Application.Dtos;
using NArchitecture.Core.Security.OAuth.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebAPI.Controllers;

/// <summary>
/// AuthController kimlik doğrulama ve yetkilendirme işlemlerini yönetir.
/// Temel Özellikler:
/// •	JWT tabanlı kimlik doğrulama
/// •	Social Login(Google ve Facebook) desteği
/// •	İki faktörlü kimlik doğrulama(2FA)
/// •	Refresh Token yönetimi
/// •	Cookie tabanlı oturum yönetimi
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseController
{
    private readonly WebApiConfiguration _configuration;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IFacebookAuthService _facebookAuthService;
    private readonly IAuthService _authService;
    private readonly IUserSessionService _sessionService;

    public AuthController(IConfiguration configuration, IGoogleAuthService googleAuthService, IFacebookAuthService facebookAuthService, IAuthService authService, IUserSessionService sessionService)
    {
        const string configurationSection = "WebAPIConfiguration";
        _configuration =
            configuration.GetSection(configurationSection).Get<WebApiConfiguration>()
            ?? throw new NullReferenceException($"\"{configurationSection}\" section cannot found in configuration.");
        _googleAuthService = googleAuthService;
        _facebookAuthService = facebookAuthService;
        _authService = authService;
        _sessionService = sessionService;
    }

    /// <summary>
    /// Login işlemi için gerekli olan bilgileri alır ve kullanıcıyı doğrular.
    /// [POST] /api/auth/login
    /// // Kullanım örneği:
    /// {
    ///     "email": "user@example.com",
    ///     "password": "yourPassword",
    ///     "authenticatorCode": "123456" // 2FA aktifse
    /// }
    /// </summary>
    /// <param name="userForLoginDto"></param>
    /// <returns></returns>
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserForLoginDto userForLoginDto)
    {
        // Oturum kaydı ve şüpheli kontrolü
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        var ua = Request.Headers["User-Agent"].ToString();

        LoginCommand loginCommand = new() { UserForLoginDto = userForLoginDto, IpAddress = getIpAddress(), UserAgent = ua };
        LoggedResponse result = await Mediator.Send(loginCommand);

        if (result.RefreshToken is not null)
            setRefreshTokenToCookie(result.RefreshToken);

        // AccessToken’dan kullanıcı ID’sini çıkart
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(result.AccessToken?.Token);
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized();


        // Oturum kaydı ekle ve şüpheli oturumları kontrol et        
        await _sessionService.AddAsync(new UserSession
        {
            UserId = userId,
            IpAddress = ip,
            UserAgent = ua,
        });
        await _authService.FlagAndHandleSuspiciousSessionsAsync(userId);

        return Ok(result.ToHttpResponse());
    }

    /// <summary>
    /// Register işlemi için gerekli olan bilgileri alır ve kullanıcıyı kaydeder.
    /// </summary>
    /// <param name="userForRegisterDto"></param>
    /// <returns></returns>
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserForRegisterDto userForRegisterDto)
    {
        RegisterCommand registerCommand = new() { UserForRegisterDto = userForRegisterDto, IpAddress = getIpAddress() };
        RegisteredResponse result = await Mediator.Send(registerCommand);
        setRefreshTokenToCookie(result.RefreshToken);
        return Created(uri: "", result.AccessToken);
    }
    /// <summary>
    /// Refresh token'ı kullanarak yeni bir access token alır.
    /// </summary>
    public class refreshTokenModel
    {
        public string? refreshToken { get; set; }
    }
    /// <summary>
    /// Refresh token'ı kullanarak yeni bir access token alır.
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] refreshTokenModel? refreshToken)
    {
        RefreshTokenCommand refreshTokenCommand =
            new() { RefreshToken = refreshToken?.refreshToken ?? getRefreshTokenFromCookies(), IpAddress = getIpAddress() };
        RefreshedTokensResponse result = await Mediator.Send(refreshTokenCommand);
        setRefreshTokenToCookie(result.RefreshToken);
        return Created(uri: "", result.AccessToken);
    }
    /// <summary>
    /// Revoke token işlemi için gerekli olan bilgileri alır ve kullanıcıdan refresh token'ı iptal eder.
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    [HttpPut("RevokeToken")]
    public async Task<IActionResult> RevokeToken([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] string? refreshToken)
    {
        RevokeTokenCommand revokeTokenCommand =
            new() { Token = refreshToken ?? getRefreshTokenFromCookies(), IpAddress = getIpAddress() };
        RevokedTokenResponse result = await Mediator.Send(revokeTokenCommand);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcıyı çıkış yapar ve ana sayfaya yönlendirir.
    /// </summary>
    /// <returns></returns>
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        // Refresh token'ı iptal et
        await RevokeToken(null);

        // Kullanıcıyı çıkış yapar ve ana sayfaya yönlendirir
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }

    /// <summary>
    /// Email tabanlı 2FA aktifleştirir
    /// </summary>
    /// <returns></returns>
    [HttpGet("EnableEmailAuthenticator")]
    public async Task<IActionResult> EnableEmailAuthenticator()
    {
        EnableEmailAuthenticatorCommand enableEmailAuthenticatorCommand =
            new()
            {
                UserId = getUserIdFromRequest(),
                VerifyEmailUrlPrefix = $"{_configuration.ApiDomain}/Auth/VerifyEmailAuthenticator"
            };
        await Mediator.Send(enableEmailAuthenticatorCommand);

        return Ok();
    }

    /// <summary>
    /// Otp tabanlı 2FA aktifleştirir
    /// </summary>
    /// <returns></returns>
    [HttpGet("EnableOtpAuthenticator")]
    public async Task<IActionResult> EnableOtpAuthenticator()
    {
        EnableOtpAuthenticatorCommand enableOtpAuthenticatorCommand = new() { UserId = getUserIdFromRequest() };
        EnabledOtpAuthenticatorResponse result = await Mediator.Send(enableOtpAuthenticatorCommand);

        return Ok(result);
    }


    /// <summary>
    /// DisableEmailAuthenticator işlemi, kullanıcının email tabanlı 2FA doğrulamasını devre dışı bırakır.
    /// </summary>
    /// <returns></returns>
    [HttpGet("DisableEmailAuthenticator")]
    public async Task<IActionResult> DisableEmailAuthenticator()
    {
        DisableEmailAuthenticatorCommand disableCommand = new() { UserId = getUserIdFromRequest() };
        await Mediator.Send(disableCommand);
        return Ok();
    }
    /// <summary>
    /// DisableOtpAuthenticator işlemi, kullanıcının Otp tabanlı 2FA doğrulamasını devre dışı bırakır.
    /// </summary>
    /// <returns></returns>
    [HttpGet("DisableOtpAuthenticator")]
    public async Task<IActionResult> DisableOtpAuthenticator()
    {
        DisableOtpAuthenticatorCommand disableCommand = new() { UserId = getUserIdFromRequest() };
        await Mediator.Send(disableCommand);
        return Ok();
    }

    /// <summary>
    /// Email tabanlı doğrulama işlemini gerçekleştirir.
    /// </summary>
    /// <param name="verifyEmailAuthenticatorCommand"></param>
    /// <returns></returns>
    [HttpGet("VerifyEmailAuthenticator")]
    public async Task<IActionResult> VerifyEmailAuthenticator(
        [FromQuery] VerifyEmailAuthenticatorCommand verifyEmailAuthenticatorCommand
    )
    {
        await Mediator.Send(verifyEmailAuthenticatorCommand);
        return Ok();
    }

    /// <summary>
    /// Otp tabanlı 2FA doğrulama işlemini gerçekleştirir.
    /// </summary>
    /// <param name="authenticatorCode"></param>
    /// <returns></returns>
    [HttpPost("VerifyOtpAuthenticator")]
    public async Task<IActionResult> VerifyOtpAuthenticator([FromBody] string authenticatorCode)
    {
        VerifyOtpAuthenticatorCommand verifyOtpAuthenticatorCommand =
            new() { UserId = getUserIdFromRequest(), ActivationCode = authenticatorCode };

        await Mediator.Send(verifyOtpAuthenticatorCommand);
        return Ok();
    }

    /// <summary>
    /// Google ile giriş yapma işlemini başlatır.
    /// </summary>
    /// <returns></returns>
    [HttpGet("google/login")]
    public IActionResult GoogleLogin()
    {
        string authUrl = _googleAuthService.GetAuthorizationUrl();
        return Redirect(authUrl);
    }

    /// <summary>
    /// Google ile giriş yapma işlemini tamamlar.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code)
    {
        var result = await _googleAuthService.AuthenticateAsync(code);
        if (!result.Success)
            return BadRequest(result.Error);

        // Oturum kaydı ve şüpheli kontrolü
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        var ua = Request.Headers["User-Agent"].ToString();


        // Kullanıcıyı sistemde kaydet/güncelle ve JWT token üret
        // Token oluştur
        var tokenResult = await _authService.CreateTokenForExternalUser(result.User!, ip, ua);

        // Refresh token'ı cookie'ye kaydet
        setRefreshTokenToCookie(tokenResult.RefreshToken);

        return Ok(tokenResult);
    }

    /// <summary>
    /// Facebook ile giriş yapma işlemini başlatır.
    /// </summary>
    /// <returns></returns>
    [HttpGet("facebook/login")]
    public IActionResult FacebookLogin()
    {
        string authUrl = _facebookAuthService.GetAuthorizationUrl();
        return Redirect(authUrl);
    }

    /// <summary>
    /// Facebook ile giriş yapma işlemini tamamlar.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpGet("facebook/callback")]
    public async Task<IActionResult> FacebookCallback([FromQuery] string code)
    {
        var result = await _facebookAuthService.AuthenticateAsync(code);
        if (!result.Success)
            return BadRequest(result.Error);

        // Oturum kaydı ve şüpheli kontrolü
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        var ua = Request.Headers["User-Agent"].ToString();


        // Kullanıcıyı sistemde kaydet/güncelle ve JWT token üret
        // Token oluştur
        var tokenResult = await _authService.CreateTokenForExternalUser(result.User!, ip, ua);

        // Refresh token'ı cookie'ye kaydet
        setRefreshTokenToCookie(tokenResult.RefreshToken);

        return Ok(tokenResult);
    }



    /// <summary>
    /// İstemciden gelen HTTP isteğindeki "refreshToken" çerezini alır.
    /// Eğer çerez bulunamazsa bir <see cref="ArgumentException"/> fırlatır.
    /// </summary>
    /// <returns>Refresh token değerini döner.</returns>
    /// <exception cref="ArgumentException">Eğer "refreshToken" çerezi bulunamazsa fırlatılır.</exception>
    private string getRefreshTokenFromCookies()
    {
        return Request.Cookies["refreshToken"] ?? throw new ArgumentException("Refresh token is not found in request cookies.");
    }

    /// <summary>
    /// Refresh token'ı istemciye güvenli bir şekilde çerez olarak ayarlar.
    /// Çerez HttpOnly olarak işaretlenir ve 7 gün boyunca geçerli olacak şekilde ayarlanır.
    /// </summary>
    /// <param name="refreshToken">Ayarlanacak refresh token nesnesi.</param>
    private void setRefreshTokenToCookie(RefreshToken refreshToken)
    {
        CookieOptions cookieOptions = new() { HttpOnly = true, Expires = DateTime.UtcNow.AddDays(7) };
        Response.Cookies.Append(key: "refreshToken", refreshToken.Token, cookieOptions);
    }
}
