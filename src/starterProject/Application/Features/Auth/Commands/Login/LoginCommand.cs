using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using Application.Services.EmergencyAndSecretServices;
using Application.Services.UsersService;
using Domain.Entities;
using MediatR;
using NArchitectureTemplate.Core.Application.Dtos;
using NArchitectureTemplate.Core.Security.Enums;
using NArchitectureTemplate.Core.Security.JWT;

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
        private readonly AuditService audit;

        public LoginCommandHandler(
            IUserService userService,
            IAuthService authService,
            AuthBusinessRules authBusinessRules,
            Services.UserSessions.IUserSessionService userSessionService,
            AuditService audit)
        {
            _userService = userService;
            _authService = authService;
            _authBusinessRules = authBusinessRules;
            this.audit = audit;
        }

        public async Task<LoggedResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userService.GetAsync(
                predicate: u => u.Email == request.UserForLoginDto.Email,
                cancellationToken: cancellationToken
            );
            await _authBusinessRules.UserShouldBeExistsWhenSelected(user);
            await _authBusinessRules.UserPasswordShouldBeMatch(user!, request.UserForLoginDto.Password);
            await _authBusinessRules.UserShouldBeActive(user!);

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

            Domain.Entities.RefreshToken createdRefreshToken = await _authService.CreateRefreshToken(user, request.IpAddress, request.UserAgent);
            Domain.Entities.RefreshToken addedRefreshToken = await _authService.AddRefreshToken(createdRefreshToken);
            await _authService.DeleteOldRefreshTokens(user.Id);

            audit.LogAccess(
            "AUTH",
            "LOGIN_SUCCESS",
            request.UserForLoginDto.Email,
            request.IpAddress
            );

            loggedResponse.AccessToken = createdAccessToken;
            loggedResponse.RefreshToken = addedRefreshToken;
            return loggedResponse;
        }
    }
}
