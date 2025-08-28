using FluentValidation;

namespace Application.Features.Users.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(c => c.Token).NotEmpty();
        RuleFor(x => x.NewPassword)
           .NotEmpty()
           .MinimumLength(4)
           .Matches(@"[A-Z]+").WithMessage("Şifre en az bir büyük harf içermelidir.")
           .Matches(@"[a-z]+").WithMessage("Şifre en az bir küçük harf içermelidir.")
           .Matches(@"[0-9]+").WithMessage("Şifre en az bir rakam içermelidir.");
    }
}