using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Profiles;
using Application.Features.Auth.Rules;
using Application.Features.Users.Rules;
using Application.Services;
using Application.Services.AuthService;
using Application.Services.Repositories;
using Application.Services.UserSessions;
using Application.Services.UsersService;
using AutoMapper;
using FluentValidation.TestHelper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using NArchitecture.Core.Localization.Resource.Yaml;
using NArchitecture.Core.Mailing;
using NArchitecture.Core.Mailing.MailKit;
using NArchitecture.Core.Security.EmailAuthenticator;
using NArchitecture.Core.Security.JWT;
using NArchitecture.Core.Security.OtpAuthenticator;
using NArchitecture.Core.Security.OtpAuthenticator.OtpNet;
using StarterProject.Application.Tests.Mocks.Configurations;
using StarterProject.Application.Tests.Mocks.FakeDatas;
using StarterProject.Application.Tests.Mocks.Repositories.Auth;
using static Application.Features.Auth.Commands.Login.LoginCommand;

namespace StarterProject.Application.Tests.Features.Auth.Commands.Login;

public class LoginTests
{
    private readonly LoginCommand _loginCommand;
    private readonly LoginCommandHandler _loginCommandHandler;
    private readonly LoginCommandValidator _validator;
    private readonly IConfiguration _configuration;
    private readonly IMediator _mediator = Mock.Of<IMediator>();

    public LoginTests(
        OperationClaimFakeData operationClaimFakeData,
        RefreshTokenFakeData refreshTokenFakeData,
        UserFakeData userFakeData
    )
    {
        #region Eksik Mock'ları Ekleyin
        // Yeni mock'lar ekleyin
        IUserSessionService _userSessionService = Mock.Of<IUserSessionService>();
        INotificationService _notificationService = Mock.Of<INotificationService>();
        #endregion
        _configuration = MockConfiguration.GetConfigurationMock();
        #region Mock Repositories
        IUserOperationClaimRepository _userOperationClaimRepository = new MockUserClaimRepository(
            operationClaimFakeData
        ).GetMockUserClaimRepository();
        IRefreshTokenRepository _refreshTokenRepository = new MockRefreshTokenRepository(
            refreshTokenFakeData
        ).GetMockRefreshTokenRepository();
        // YENİ EKLENEN KISIM: UserRoleRepository mock
        IUserRoleRepository _userRoleRepository = Mock.Of<IUserRoleRepository>(); // <-- Bu satırı ekleyin
        IEmailAuthenticatorRepository _userEmailAuthenticatorRepository =
            MockEmailAuthenticatorRepository.GetEmailAuthenticatorRepositoryMock();
        IOtpAuthenticatorRepository _userOtpAuthenticatorRepository = MockOtpAuthRepository.GetOtpAuthenticatorMock();
        IUserRepository _userRepository = new MockUserRepository(userFakeData).GetUserMockRepository();
        #endregion
        #region Mock Helpers
        TokenOptions tokenOptions =
            _configuration.GetSection("TokenOptions").Get<TokenOptions>() ?? throw new Exception("Token options not found.");
        ITokenHelper<Guid, int, int, Guid> tokenHelper = new JwtHelper<Guid, int, int, Guid>(tokenOptions);
        IEmailAuthenticatorHelper emailAuthenticatorHelper = new EmailAuthenticatorHelper();
        MailSettings mailSettings =
            _configuration.GetSection("MailSettings").Get<MailSettings>() ?? throw new Exception("Mail settings not found.");
        IMailService mailService = new MailKitMailService(mailSettings);
        IOtpAuthenticatorHelper otpAuthenticatorHelper = new OtpNetOtpAuthenticatorHelper();
        ILocalizationService localizationService = new ResourceLocalizationService(resources: [])
        {
            AcceptLocales = new[] { "tr" }
        };

        var config = new MapperConfiguration(cfg => {
            cfg.AddProfile<MappingProfiles>();
        }, NullLoggerFactory.Instance); // ILoggerFactory parametresi eklendi.

        IMapper mapper = config.CreateMapper();

        #endregion
        AuthBusinessRules authBusinessRules = new(_userRepository, localizationService);
        UserBusinessRules _userBusinessRules = new(_userRepository, localizationService);
        IUserService _userService = new UserService(_userRepository, _userBusinessRules);
        IAuthService _authService = new AuthService(
                   _userOperationClaimRepository,
                   _refreshTokenRepository,
                   tokenHelper,
                   _configuration,
                   mapper,
                   mailService,
                   _userOtpAuthenticatorRepository,
                   otpAuthenticatorHelper,
                   _userEmailAuthenticatorRepository,
                   emailAuthenticatorHelper,
                   _userRepository,
                   _userService,
                   _mediator,
                   _userSessionService, // EKLENDİ
                   _notificationService,  // EKLENDİ
                     _userRoleRepository // EKLENDİ: UserRoleRepository ekleniyor
               );
        IAuthService _authenticatorService = new AuthService(
            _userOperationClaimRepository,
            _refreshTokenRepository,
            tokenHelper,
            _configuration,
            mapper,
            mailService,
            _userOtpAuthenticatorRepository,
            otpAuthenticatorHelper,
            _userEmailAuthenticatorRepository,
            emailAuthenticatorHelper,
            _userRepository,
            _userService,
            _mediator,
            _userSessionService, // EKLENDİ
            _notificationService,  // EKLENDİ
            _userRoleRepository // EKLENDİ: UserRoleRepository ekleniyor

        );
        _validator = new LoginCommandValidator();
        _loginCommand = new LoginCommand();
        _loginCommandHandler = new LoginCommandHandler(_userService, _authService, authBusinessRules, _userSessionService);
    }

    [Fact]
    public async Task SuccessfulLoginShouldReturnAccessToken()
    {
        _loginCommand.UserForLoginDto = new() { Email = "example@serdarsevimli.tr", Password = "123456" };
        LoggedResponse result = await _loginCommandHandler.Handle(_loginCommand, CancellationToken.None);
        Assert.NotNull(result.AccessToken?.Token);
    }

    [Fact]
    public async Task AccessTokenShouldHaveValidExpirationTime()
    {
        _loginCommand.UserForLoginDto = new() { Email = "example@serdarsevimli.tr", Password = "123456" };
        LoggedResponse result = await _loginCommandHandler.Handle(_loginCommand, CancellationToken.None);
        TokenOptions? tokenOptions = _configuration.GetSection("TokenOptions").Get<TokenOptions>();
        bool tokenExpiresInTime =
            DateTime.Now.AddMinutes(tokenOptions!.AccessTokenExpiration + 1) > result.AccessToken!.ExpirationDate;
        Assert.True(tokenExpiresInTime, "Access token expiration time is invalid.");
    }

    [Fact]
    public async Task LoginWithWrongPasswordShouldThrowException()
    {
        _loginCommand.UserForLoginDto = new() { Email = "example@serdarsevimli.tr", Password = "123456789" };
        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _loginCommandHandler.Handle(_loginCommand, CancellationToken.None);
        });
    }

    [Fact]
    public async Task LoginWithWrongEmailShouldThrowException()
    {
        _loginCommand.UserForLoginDto = new() { Email = "halit1@serdarsevimli.tr", Password = "123456" };
        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _loginCommandHandler.Handle(_loginCommand, CancellationToken.None);
        });
    }

    [Fact]
    public void LoginWithInvalidLengthPasswordShouldThrowException()
    {
        _loginCommand.UserForLoginDto = new() { Email = "halit1@serdarsevimli.tr", Password = "1" };
        TestValidationResult<LoginCommand> validationResult = _validator.TestValidate(_loginCommand);
        validationResult.ShouldHaveValidationErrorFor(i => i.UserForLoginDto.Password);
    }

    [Fact]
    public void LoginWithNullPasswordShouldThrowException()
    {
        _loginCommand.UserForLoginDto = new() { Email = "halit1@serdarsevimli.tr", Password = null! };
        TestValidationResult<LoginCommand> validationResult = _validator.TestValidate(_loginCommand);
        validationResult.ShouldHaveValidationErrorFor(i => i.UserForLoginDto.Password);
    }
}
