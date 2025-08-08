using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using Application.Services.UserSessions;
using Application.Services.UsersService;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Dtos;
using NArchitecture.Core.Security.Enums;
using NArchitecture.Core.Security.JWT;

namespace Application.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<LoggedResponse>
{
    public UserForLoginDto UserForLoginDto { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }

    public LoginCommand()
    {
        UserForLoginDto = null!;
        IpAddress = string.Empty;
        UserAgent = string.Empty;
    }

    public LoginCommand(UserForLoginDto userForLoginDto, string ipAddress, string userAgent)
    {
        UserForLoginDto = userForLoginDto;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoggedResponse>
    {
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IUserSessionService _sessionService;
        private IUserService _userService1;
        private IAuthService _authService1;
        private AuthBusinessRules _authBusinessRules1;

        public LoginCommandHandler(IUserService userService1, IAuthService authService1, AuthBusinessRules authBusinessRules1)
        {
            _userService1 = userService1;
            _authService1 = authService1;
            _authBusinessRules1 = authBusinessRules1;
        }

        public LoginCommandHandler(
            IUserService userService,
            IAuthService authService,
            AuthBusinessRules authBusinessRules
,
            IUserSessionService sessionService)
        {
            _userService = userService;
            _authService = authService;
            _authBusinessRules = authBusinessRules;
            _sessionService = sessionService;
        }

        public async Task<LoggedResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userService.GetAsync(
                predicate: u => u.Email == request.UserForLoginDto.Email,
                cancellationToken: cancellationToken
            );
            await _authBusinessRules.UserShouldBeExistsWhenSelected(user);
            await _authBusinessRules.UserPasswordShouldBeMatch(user!, request.UserForLoginDto.Password);

            LoggedResponse loggedResponse = new();

            if (user!.AuthenticatorType is not AuthenticatorType.None)
            {
                if (request.UserForLoginDto.AuthenticatorCode is null)
                {
                    await _authService.SendAuthenticatorCode(user);
                    loggedResponse.RequiredAuthenticatorType = user.AuthenticatorType;
                    return loggedResponse;
                }

                await _authService.VerifyAuthenticatorCode(user, request.UserForLoginDto.AuthenticatorCode);
            }

            AccessToken createdAccessToken = await _authService.CreateAccessToken(user);

            Domain.Entities.RefreshToken createdRefreshToken = await _authService.CreateRefreshToken(user, request.IpAddress,request.UserAgent);
            Domain.Entities.RefreshToken addedRefreshToken = await _authService.AddRefreshToken(createdRefreshToken);
            await _authService.DeleteOldRefreshTokens(user.Id);

            loggedResponse.AccessToken = createdAccessToken;
            loggedResponse.RefreshToken = addedRefreshToken;
            return loggedResponse;
        }
    }
}
