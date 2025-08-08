using System.Web;
using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using Application.Services.Repositories;
using Application.Services.UsersService;
using Domain.Entities;
using MediatR;
using MimeKit;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Mailing;
using NArchitecture.Core.Security.Enums;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Auth.Commands.EnableEmailAuthenticator;

public class EnableEmailAuthenticatorCommand : IRequest, ISecuredRequest
{
    public Guid UserId { get; set; }
    public string VerifyEmailUrlPrefix { get; set; }

    public string[] Roles => [];

    public EnableEmailAuthenticatorCommand()
    {
        VerifyEmailUrlPrefix = string.Empty;
    }

    public EnableEmailAuthenticatorCommand(Guid userId, string verifyEmailUrlPrefix)
    {
        UserId = userId;
        VerifyEmailUrlPrefix = verifyEmailUrlPrefix;
    }

    public class EnableEmailAuthenticatorCommandHandler : IRequestHandler<EnableEmailAuthenticatorCommand>
    {
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IAuthService _authenticatorService;
        private readonly IEmailAuthenticatorRepository _emailAuthenticatorRepository;
        private readonly IMailService _mailService;
        private readonly IUserService _userService;
        private readonly string _appName;

        public EnableEmailAuthenticatorCommandHandler(
            IUserService userService,
            IEmailAuthenticatorRepository emailAuthenticatorRepository,
            IMailService mailService,
            AuthBusinessRules authBusinessRules,
            IAuthService authenticatorService,
            IConfiguration configuration
        )
        {
            _userService = userService;
            _emailAuthenticatorRepository = emailAuthenticatorRepository;
            _mailService = mailService;
            _authBusinessRules = authBusinessRules;
            _authenticatorService = authenticatorService;
            _appName = configuration.GetValue<string>("AppName")
                ?? throw new NullReferenceException("\"AppName\" bölümü konfigürasyonda bulunamadı.");
        }

        public async Task Handle(EnableEmailAuthenticatorCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userService.GetAsync(
                predicate: u => u.Id == request.UserId,
                cancellationToken: cancellationToken
            );
            await _authBusinessRules.UserShouldBeExistsWhenSelected(user);
            await _authBusinessRules.UserShouldNotBeHaveAuthenticator(user!);

            user!.AuthenticatorType = AuthenticatorType.Email;
            await _userService.UpdateAsync(user);

            EmailAuthenticator emailAuthenticator = await _authenticatorService.CreateEmailAuthenticator(user);
            EmailAuthenticator addedEmailAuthenticator = await _emailAuthenticatorRepository.AddAsync(emailAuthenticator);

            var toEmailList = new List<MailboxAddress> { new(name: user.Email, user.Email) };

            _mailService.SendMail(
    new Mail
    {
        ToList = toEmailList,
        Subject = $"E-postanızı Doğrulayın - {_appName}",
        HtmlBody = $@"
            <html>
            <body>
                <p>E-postanızı doğrulamak için lütfen aşağıdaki bağlantıya tıklayın:</p>
                <p>
                    <a href=""{request.VerifyEmailUrlPrefix}?ActivationKey={ConvertToUrlSafeBase64(addedEmailAuthenticator.ActivationKey!)}"">
                        Tıklayın
                    </a>
                </p>
                <p>Eğer bu işlemi siz başlatmadıysanız, bu e-postayı dikkate almayınız.</p>
            </body>
            </html>"
    }
);
        }
        private string ConvertToUrlSafeBase64(string base64)
        {
            return base64
                .Replace('+', '-') // URL'de sorun çıkaran +
                .Replace('/', '_') // URL'de sorun çıkaran /
                .Replace("=", ""); // Padding karakterlerini kaldır
        }
    }
}