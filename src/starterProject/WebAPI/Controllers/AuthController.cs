using Application.Features.Auth.Commands.EnableEmailAuthenticator;
using Application.Features.Auth.Commands.EnableOtpAuthenticator;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Commands.RevokeToken;
using Application.Features.Auth.Commands.VerifyEmailAuthenticator;
using Application.Features.Auth.Commands.VerifyOtpAuthenticator;
using Application.Services.AuthService;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NArchitecture.Core.Application.Dtos;
using NArchitecture.Core.Security.OAuth.Services;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseController
{
    private readonly WebApiConfiguration _configuration;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IFacebookAuthService _facebookAuthService;
    private readonly IAuthService _authService;

    public AuthController(IConfiguration configuration, IGoogleAuthService googleAuthService, IFacebookAuthService facebookAuthService, IAuthService authService)
    {
        const string configurationSection = "WebAPIConfiguration";
        _configuration =
            configuration.GetSection(configurationSection).Get<WebApiConfiguration>()
            ?? throw new NullReferenceException($"\"{configurationSection}\" section cannot found in configuration.");
        _googleAuthService = googleAuthService;
        _facebookAuthService = facebookAuthService;
        _authService = authService;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserForLoginDto userForLoginDto)
    {
        LoginCommand loginCommand = new() { UserForLoginDto = userForLoginDto, IpAddress = getIpAddress() };
        LoggedResponse result = await Mediator.Send(loginCommand);

        if (result.RefreshToken is not null)
            setRefreshTokenToCookie(result.RefreshToken);

        return Ok(result.ToHttpResponse());
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserForRegisterDto userForRegisterDto)
    {
        RegisterCommand registerCommand = new() { UserForRegisterDto = userForRegisterDto, IpAddress = getIpAddress() };
        RegisteredResponse result = await Mediator.Send(registerCommand);
        setRefreshTokenToCookie(result.RefreshToken);
        return Created(uri: "", result.AccessToken);
    }

    public class refreshTokenModel
    {
        public string? refreshToken { get; set; }
    }
    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] refreshTokenModel? refreshToken)
    {
        RefreshTokenCommand refreshTokenCommand =
            new() { RefreshToken = refreshToken?.refreshToken ?? getRefreshTokenFromCookies(), IpAddress = getIpAddress() };
        RefreshedTokensResponse result = await Mediator.Send(refreshTokenCommand);
        setRefreshTokenToCookie(result.RefreshToken);
        return Created(uri: "", result.AccessToken);
    }

    [HttpPut("RevokeToken")]
    public async Task<IActionResult> RevokeToken([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] string? refreshToken)
    {
        RevokeTokenCommand revokeTokenCommand =
            new() { Token = refreshToken ?? getRefreshTokenFromCookies(), IpAddress = getIpAddress() };
        RevokedTokenResponse result = await Mediator.Send(revokeTokenCommand);
        return Ok(result);
    }

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

    [HttpGet("EnableOtpAuthenticator")]
    public async Task<IActionResult> EnableOtpAuthenticator()
    {
        EnableOtpAuthenticatorCommand enableOtpAuthenticatorCommand = new() { UserId = getUserIdFromRequest() };
        EnabledOtpAuthenticatorResponse result = await Mediator.Send(enableOtpAuthenticatorCommand);

        return Ok(result);
    }

    [HttpGet("VerifyEmailAuthenticator")]
    public async Task<IActionResult> VerifyEmailAuthenticator(
        [FromQuery] VerifyEmailAuthenticatorCommand verifyEmailAuthenticatorCommand
    )
    {
        await Mediator.Send(verifyEmailAuthenticatorCommand);
        return Ok();
    }

    [HttpPost("VerifyOtpAuthenticator")]
    public async Task<IActionResult> VerifyOtpAuthenticator([FromBody] string authenticatorCode)
    {
        VerifyOtpAuthenticatorCommand verifyEmailAuthenticatorCommand =
            new() { UserId = getUserIdFromRequest(), ActivationCode = authenticatorCode };

        await Mediator.Send(verifyEmailAuthenticatorCommand);
        return Ok();
    }

    [HttpGet("google/login")]
    public IActionResult GoogleLogin()
    {
        string authUrl = _googleAuthService.GetAuthorizationUrl();
        return Redirect(authUrl);
    }

    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code)
    {
        var result = await _googleAuthService.AuthenticateAsync(code);
        if (!result.Success)
            return BadRequest(result.Error);

        // Kullanıcıyı sistemde kaydet/güncelle ve JWT token üret
        // Token oluştur
        var tokenResult = await _authService.CreateTokenForExternalUser(result.User);

        // Refresh token'ı cookie'ye kaydet
        setRefreshTokenToCookie(tokenResult.RefreshToken);

        return Ok(result.User);
    }

    [HttpGet("facebook/login")]
    public IActionResult FacebookLogin()
    {
        string authUrl = _facebookAuthService.GetAuthorizationUrl();
        return Redirect(authUrl);
    }

    [HttpGet("facebook/callback")]
    public async Task<IActionResult> FacebookCallback([FromQuery] string code)
    {
        var result = await _facebookAuthService.AuthenticateAsync(code);
        if (!result.Success)
            return BadRequest(result.Error);

        // Kullanıcıyı sistemde kaydet/güncelle ve JWT token üret
        return Ok(result.User);
    }
    private string getRefreshTokenFromCookies()
    {
        return Request.Cookies["refreshToken"] ?? throw new ArgumentException("Refresh token is not found in request cookies.");
    }

    private void setRefreshTokenToCookie(RefreshToken refreshToken)
    {
        CookieOptions cookieOptions = new() { HttpOnly = true, Expires = DateTime.UtcNow.AddDays(7) };
        Response.Cookies.Append(key: "refreshToken", refreshToken.Token, cookieOptions);
    }
}
