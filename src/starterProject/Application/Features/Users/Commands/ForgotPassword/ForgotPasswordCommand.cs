using Application.Services.Repositories;
using Application.Services.UsersService;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.Types;
using NArchitectureTemplate.Core.Mailing;

namespace Application.Features.Users.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest<ForgotPasswordResponse>
{
    public string Email { get; set; }

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
    {
        private readonly IUserService _userService;
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
        private readonly IMailService _mailService;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly FrontedConfig _resetPasswordSettings;


        public ForgotPasswordCommandHandler(IMailService mailService, IPasswordResetTokenRepository passwordResetTokenRepository, IHostEnvironment hostEnvironment, IUserService userService, IOptions<FrontedConfig> resetPasswordSettings)
        {
            _mailService = mailService;
            _passwordResetTokenRepository = passwordResetTokenRepository;
            _hostEnvironment = hostEnvironment;
            _userService = userService;
            _resetPasswordSettings = resetPasswordSettings.Value;
        }

        public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userService.GetAsync(u => u.Email == request.Email);
            if (user == null)
            {
                throw new BusinessException("Kullanýcý bulunamadý.");
            }
            string token = Guid.NewGuid().ToString("N"); // Bu token'ý veritabanýnda saklamalýsýnýz.
            var expirationDate = DateTime.Now.AddDays(1);

            var resetToken = new ResetPasswordToken
            {
                Token = token,
                UserId = user.Id,
                ExpirationDate = expirationDate
            };
            await _passwordResetTokenRepository.AddAsync(resetToken);

            var toEmailList = new List<MailboxAddress> { new(name: user.Email, user.Email) };

            string baseUrl = _hostEnvironment.IsDevelopment()
                ? _resetPasswordSettings.Development
                : _resetPasswordSettings.Production;

            string resetLink = $"{baseUrl}/auth/reset-password?token={token}";
            // Modern e-posta þablonu
            string htmlBody = $@"
            <!DOCTYPE html>
            <html lang=""tr"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Þifre Sýfýrlama</title>
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
                        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
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
                    .button-container {{
                        text-align: center;
                        margin: 30px 0;
                    }}
                    .reset-button {{
                        display: inline-block;
                        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                        color: white;
                        text-decoration: none;
                        padding: 14px 35px;
                        border-radius: 5px;
                        font-weight: 600;
                        font-size: 16px;
                        transition: all 0.3s ease;
                    }}
                    .reset-button:hover {{
                        background: linear-gradient(135deg, #5a6fd8 0%, #6a4190 100%);
                        transform: translateY(-2px);
                        box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
                    }}
                    .alternate-link {{
                        word-break: break-all;
                        color: #667eea;
                        text-decoration: none;
                        font-size: 14px;
                    }}
                    .footer {{
                        text-align: center;
                        padding: 20px;
                        background-color: #f9f9f9;
                        font-size: 14px;
                        color: #666666;
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
                        <h1>Þifre Sýfýrlama</h1>
                    </div>
                    <div class=""content"">
                        <div class=""logo"">
                            <!-- Logo eklemek isterseniz -->
                            <!-- <img src=""https://your-site.tr/logo.png"" alt=""Logo""> -->
                        </div>
                        
                        <p class=""greeting"">Merhaba,</p>
                        
                        <p class=""message"">
                            Þifre sýfýrlama talebinde bulundunuz. Aþaðýdaki butona týklayarak 
                            yeni þifrenizi oluþturabilirsiniz. Bu link 24 saat boyunca geçerlidir.
                        </p>
                        
                        <div class=""button-container"">
                            <a href=""{resetLink}"" class=""reset-button"">Þifremi Sýfýrla</a>
                        </div>
                        
                        <div class=""warning"">
                            <strong>Önemli:</strong> Eðer bu talebi siz yapmadýysanýz, 
                            lütfen bu e-postayý dikkate almayýn ve hesabýnýzý kontrol edin.
                        </div>
                        
                        <p class=""message"">
                            Buton çalýþmýyorsa, aþaðýdaki linki tarayýcýnýza kopyalayýp yapýþtýrabilirsiniz:
                            <br><br>
                            <a href=""{resetLink}"" class=""alternate-link"">{resetLink}</a>
                        </p>
                    </div>
                    <div class=""footer"">
                        <p>© {DateTime.Now.Year} Þirket Adý. Tüm haklarý saklýdýr.</p>
                        <p>Bu e-posta otomatik olarak gönderilmiþtir, lütfen yanýtlamayýn.</p>
                    </div>
                </div>
            </body>
            </html>";

            _mailService.SendMail(
                new Mail
                {
                    ToList = toEmailList,
                    Subject = "Þifre Sýfýrlama Talebi",
                    HtmlBody = htmlBody
                }
            );

            return new ForgotPasswordResponse { IsSuccess = true, Message = "Þifre sýfýrlama e-postasý gönderildi." };


        }
    }
}
