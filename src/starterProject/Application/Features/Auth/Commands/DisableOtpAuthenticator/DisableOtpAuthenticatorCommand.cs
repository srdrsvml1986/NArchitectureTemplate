using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using Application.Services.Repositories;
using Application.Services.UsersService;
using Domain.Entities;
using MediatR;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Security.Enums;

namespace Application.Features.Auth.Commands.DisableOtpAuthenticator;

public class DisableOtpAuthenticatorCommand : IRequest, IRequestAdvancedAuthorization
{
    public Guid UserId { get; set; }

    public string[] Roles => ["User"];

    public string[] Permissions => [];

    public string[] Groups => [];

    public class DisableOtpAuthenticatorCommandHandler : IRequestHandler<DisableOtpAuthenticatorCommand>
    {
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IOtpAuthenticatorRepository _otpAuthenticatorRepository;
        private readonly IUserService _userService;

        public DisableOtpAuthenticatorCommandHandler(
            IUserService userService,
            IOtpAuthenticatorRepository otpAuthenticatorRepository,
            AuthBusinessRules authBusinessRules
        )
        {
            _userService = userService;
            _otpAuthenticatorRepository = otpAuthenticatorRepository;
            _authBusinessRules = authBusinessRules;
        }

        public async Task Handle(DisableOtpAuthenticatorCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userService.GetAsync(
                predicate: u => u.Id == request.UserId,
                cancellationToken: cancellationToken
            );
            await _authBusinessRules.UserShouldBeExistsWhenSelected(user);
            await _authBusinessRules.UserShouldNotBeHaveAuthenticator(user!);

            // OTP kaydını sil
            OtpAuthenticator? otpAuthenticator = await _otpAuthenticatorRepository.GetAsync(
                o => o.UserId == request.UserId,
                cancellationToken: cancellationToken
            );
            if (otpAuthenticator is not null)
                await _otpAuthenticatorRepository.DeleteAsync(otpAuthenticator);

            // Kullanıcıyı güncelle
            user!.AuthenticatorType = AuthenticatorType.None;
            await _userService.UpdateAsync(user);
        }
    }
}