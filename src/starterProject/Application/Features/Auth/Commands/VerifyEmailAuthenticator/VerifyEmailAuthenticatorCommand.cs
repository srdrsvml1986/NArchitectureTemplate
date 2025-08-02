using Application.Features.Auth.Rules;
using Application.Services.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Commands.VerifyEmailAuthenticator;

public class VerifyEmailAuthenticatorCommand : IRequest
{
    public string ActivationKey { get; set; }

    public VerifyEmailAuthenticatorCommand()
    {
        ActivationKey = string.Empty;
    }

    public VerifyEmailAuthenticatorCommand(string activationKey)
    {
        ActivationKey = activationKey;
    }

    public class VerifyEmailAuthenticatorCommandHandler : IRequestHandler<VerifyEmailAuthenticatorCommand>
    {
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IEmailAuthenticatorRepository _emailAuthenticatorRepository;

        public VerifyEmailAuthenticatorCommandHandler(
            IEmailAuthenticatorRepository emailAuthenticatorRepository,
            AuthBusinessRules authBusinessRules
        )
        {
            _emailAuthenticatorRepository = emailAuthenticatorRepository;
            _authBusinessRules = authBusinessRules;
        }

        public async Task Handle(VerifyEmailAuthenticatorCommand request, CancellationToken cancellationToken)
        {
            EmailAuthenticator? emailAuthenticator = await _emailAuthenticatorRepository.GetAsync(
                predicate: e => e.ActivationKey == ConvertFromUrlSafeBase64(request.ActivationKey),
                cancellationToken: cancellationToken
            );
            await _authBusinessRules.EmailAuthenticatorShouldBeExists(emailAuthenticator);
            await _authBusinessRules.EmailAuthenticatorActivationKeyShouldBeExists(emailAuthenticator!);

            emailAuthenticator!.ActivationKey = null;
            emailAuthenticator.IsVerified = true;
            await _emailAuthenticatorRepository.UpdateAsync(emailAuthenticator);
        }
        private string ConvertFromUrlSafeBase64(string urlSafeBase64)
        {
            string base64 = urlSafeBase64
                .Replace('-', '+')
                .Replace('_', '/');

            // Padding ekle (Base64 uzunluğu 4'ün katı olmalı)
            int padding = base64.Length % 4;
            if (padding > 0)
                base64 += new string('=', 4 - padding);

            return base64;
        }
    }
}
