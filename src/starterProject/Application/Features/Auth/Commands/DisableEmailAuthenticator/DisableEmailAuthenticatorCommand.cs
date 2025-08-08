using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using Application.Services.Repositories;
using Application.Services.UsersService;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Security.Enums;

namespace Application.Features.Auth.Commands.DisableEmailAuthenticator;

public class DisableEmailAuthenticatorCommand : IRequest, ISecuredRequest
{
    public Guid UserId { get; set; }

    public string[] Roles => [];

    public class DisableEmailAuthenticatorCommandHandler : IRequestHandler<DisableEmailAuthenticatorCommand>
    {
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IAuthService _authenticatorService;
        private readonly IEmailAuthenticatorRepository _emailAuthenticatorRepository;
        private readonly IUserService _userService;

        public DisableEmailAuthenticatorCommandHandler(
            IUserService userService,
            IEmailAuthenticatorRepository emailAuthenticatorRepository,
            AuthBusinessRules authBusinessRules,
            IAuthService authenticatorService
        )
        {
            _userService = userService;
            _emailAuthenticatorRepository = emailAuthenticatorRepository;
            _authBusinessRules = authBusinessRules;
            _authenticatorService = authenticatorService;
        }

        public async Task Handle(DisableEmailAuthenticatorCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userService.GetAsync(
                predicate: u => u.Id == request.UserId,
                cancellationToken: cancellationToken
            );
            await _authBusinessRules.UserShouldBeExistsWhenSelected(user);
            await _authBusinessRules.UserShouldNotBeHaveAuthenticator(user!);

            // Email kaydını sil
            EmailAuthenticator? emailAuthenticator = await _emailAuthenticatorRepository.GetAsync(
                e => e.UserId == request.UserId,
                cancellationToken: cancellationToken
            );
            if (emailAuthenticator is not null)
                await _emailAuthenticatorRepository.DeleteAsync(emailAuthenticator);

            // Kullanıcıyı güncelle
            user!.AuthenticatorType = AuthenticatorType.None;
            await _userService.UpdateAsync(user);
        }
    }
}