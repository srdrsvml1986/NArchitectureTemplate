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
                throw new BusinessException("Jeton bulunamadı.");
            }
            if (passwordResetToken.ExpirationDate < DateTime.Now)
            {
                throw new BusinessException("Jetonun süresi doldu. Lütfen yeniden şifremi unuttum butonuna tıklayınız");
            }
            User? user = await _userRepository.GetAsync(u => u.Id==passwordResetToken.UserId);
            if (user == null)
            {
                throw new BusinessException("Kullanıcı bulunamadı.");
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
            // Modern e-posta şablonu
            string htmlBody = $@"
            <!DOCTYPE html>
            <html lang=""tr"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Şifre Değişikliği Onayı</title>
                <style>
                    body {{
                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                        background-color: #f7f7f7;
                        margin: 0;
                        padding: 0;
                        color: #333333;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 0 auto;
                        background-color: #ffffff;
                        border-radius: 8px;
                        overflow: hidden;
                        box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
                    }}
                    .header {{
                        background: linear-gradient(135deg, #4CAF50 0%, #2E7D32 100%);
                        padding: 30px 20px;
                        text-align: center;
                    }}
                    .header h1 {{
                        color: white;
                        margin: 0;
                        font-size: 28px;
                        font-weight: 600;
                    }}
                    .content {{
                        padding: 30px;
                    }}
                    .greeting {{
                        font-size: 18px;
                        margin-bottom: 20px;
                    }}
                    .message {{
                        font-size: 16px;
                        line-height: 1.6;
                        margin-bottom: 25px;
                    }}
                    .success-icon {{
                        text-align: center;
                        margin: 20px 0;
                        color: #4CAF50;
                        font-size: 64px;
                    }}
                    .footer {{
                        text-align: center;
                        padding: 20px;
                        background-color: #f9f9f9;
                        font-size: 14px;
                        color: #666666;
                    }}
                    .security-note {{
                        background-color: #e8f5e9;
                        border-left: 4px solid #4CAF50;
                        padding: 12px 15px;
                        margin: 20px 0;
                        border-radius: 4px;
                        font-size: 14px;
                    }}
                    .warning {{
                        background-color: #fff3e0;
                        border-left: 4px solid #ff9800;
                        padding: 12px 15px;
                        margin: 20px 0;
                        border-radius: 4px;
                        font-size: 14px;
                    }}
                    .logo {{
                        text-align: center;
                        margin-bottom: 20px;
                    }}
                    .logo img {{
                        max-width: 180px;
                        height: auto;
                    }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <div class=""header"">
                        <h1>Şifre Değişikliği Onayı</h1>
                    </div>
                    <div class=""content"">
                        <div class=""logo"">
                            <!-- Logo eklemek isterseniz -->
                            <!-- <img src=""https://your-site.tr/logo.png"" alt=""Logo""> -->
                        </div>
                        
                        <p class=""greeting"">Merhaba {user.FirstName} {user.LastName},</p>
                        
                        <div class=""success-icon"">
                            ✓
                        </div>
                        
                        <p class=""message"">
                            Hesabınızın şifresi başarıyla güncellenmiştir. 
                            Güvenliğiniz için şifrenizi düzenli olarak güncellemenizi öneririz.
                        </p>
                        
                        <div class=""security-note"">
                            <strong>Güvenlik Notu:</strong> Şifrenizi kimseyle paylaşmayın ve 
                            güvenli bir yerde saklayın.
                        </div>
                        
                        <div class=""warning"">
                            <strong>Önemli:</strong> Eğer bu işlemi siz yapmadıysanız, 
                            lütfen hemen <a href=""https://your-site.tr/contact"">müşteri hizmetlerimizle</a> iletişime geçin.
                        </div>
                    </div>
                    <div class=""footer"">
                        <p>© {DateTime.Now.Year} Şirket Adı. Tüm hakları saklıdır.</p>
                        <p>Bu e-pota otomatik olarak gönderilmiştir, lütfen yanıtlamayın.</p>
                    </div>
                </div>
            </body>
            </html>";

            _mailService.SendMail(
                new Mail
                {
                    ToList = toEmailList,
                    Subject = "Şifreniz Başarıyla Güncellendi",
                    HtmlBody = htmlBody
                }
            );
            return new ResetPasswordResponse { IsSuccess = true, Message = "Şifre sıfırlandı" };

        }
    }
}
