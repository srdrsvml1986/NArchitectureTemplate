using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using MimeKit;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Mailing;

namespace Application.Features.Users.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest<ForgotPasswordResponse>
{
    public string Email { get; set; }

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
        private readonly IMailService _mailService;
        private readonly IHostEnvironment _hostEnvironment;


        public ForgotPasswordCommandHandler(IUserRepository userRepository, IMailService mailService, IPasswordResetTokenRepository passwordResetTokenRepository, IHostEnvironment hostEnvironment)
        {
            _userRepository = userRepository;
            _mailService = mailService;
            _passwordResetTokenRepository = passwordResetTokenRepository;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetAsync(u => u.Email == request.Email);
            if (user == null)
            {
                throw new BusinessException("Kullan�c� bulunamad�.");
            }
            string token = Guid.NewGuid().ToString("N"); // Bu token'� veritaban�nda saklamal�s�n�z.
            var expirationDate = DateTime.Now.AddDays(1);

            var resetToken = new ResetPasswordToken
            {
                Token = token,
                UserId = user.Id,
                ExpirationDate = expirationDate
            };
            await _passwordResetTokenRepository.AddAsync(resetToken);

            var toEmailList = new List<MailboxAddress> { new(name: user.Email, user.Email) };
            string resetLink;

            if (_hostEnvironment.IsDevelopment())
            {
                resetLink = $"https://localhost:4200/auth/reset-password?token={token}";
            }
            else
            {
                resetLink = $"<a href='https://your-site.tr/auth/reset-password?token={token}'>�ifremi s�f�rla</a>";
            }
            _mailService.SendMail(
                new Mail
                {
                    ToList = toEmailList,
                    Subject = "�ifre S�f�rlama",
                    HtmlBody = $"�ifre S�f�rlama, �ifrenizi s�f�rlamak i�in t�klay�n: {resetLink}"
                }
            );
            return new ForgotPasswordResponse { IsSuccess = true, Message = "�ifre s�f�rlama e-postas� g�nderildi." };
        }
    }
}
