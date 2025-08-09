using Application.Services.Repositories;
using Domain.Entities;
using MediatR;
using MimeKit;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.Types;
using NArchitectureTemplate.Core.Mailing;
using NArchitectureTemplate.Core.Security.Hashing;

namespace Application.Features.Users.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest<ResetPasswordResponse>
{
    public string Token { get; set; }
    public string NewPassword { get; set; }

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
        private readonly IMailService _mailService;

        public ResetPasswordCommandHandler(IUserRepository userRepository, IMailService mailService, IPasswordResetTokenRepository passwordResetTokenRepository)
        {
            _userRepository = userRepository;
            _mailService = mailService;
            _passwordResetTokenRepository = passwordResetTokenRepository;
        }

        public async Task<ResetPasswordResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            ResetPasswordToken? passwordResetToken = await _passwordResetTokenRepository.GetAsync(u => u.Token == request.Token);
            if (passwordResetToken == null)
            {
                throw new BusinessException("Jeton bulunamad�.");
            }
            if (passwordResetToken.ExpirationDate < DateTime.Now)
            {
                throw new BusinessException("Jetonun s�resi doldu. L�tfen yeniden �ifremi unuttum butonuna t�klay�n�z");
            }
            User? user = await _userRepository.GetAsync(u => u.Id==passwordResetToken.UserId);
            if (user == null)
            {
                throw new BusinessException("Kullan�c� bulunamad�.");
            }

            await _passwordResetTokenRepository.DeleteAsync(passwordResetToken);

            if (request.NewPassword != null && !string.IsNullOrWhiteSpace(request.NewPassword))
            {
                HashingHelper.CreatePasswordHash(
                    request.NewPassword,
                    passwordHash: out byte[] passwordHash,
                    passwordSalt: out byte[] passwordSalt
                );
                user!.PasswordHash = passwordHash;
                user!.PasswordSalt = passwordSalt;
            }

            await _userRepository.UpdateAsync(user!);
            var toEmailList = new List<MailboxAddress> { new(name: user.Email, user.Email) };
            _mailService.SendMail(
                new Mail
                {
                    ToList = toEmailList,
                    Subject = "�ifre S�f�rlama",
                    TextBody = $"�ifreniz ba�ar� ile s�f�rland�, yeni �ifreniz: {request.NewPassword}"
                }
            );
            return new ResetPasswordResponse { IsSuccess = true, Message = "�ifre s�f�rland�" };

        }
    }
}
