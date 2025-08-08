using FluentValidation;

namespace Application.Features.Users.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(c => c.Token).NotEmpty();
        RuleFor(c => c.NewPassword).NotEmpty();
    }
}