using FluentValidation;

namespace Application.Features.UserSessions.Commands.Create;

public class CreateUserSessionCommandValidator : AbstractValidator<CreateUserSessionCommand>
{
    public CreateUserSessionCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.IpAddress).NotEmpty();
        RuleFor(c => c.UserAgent).NotEmpty();
        RuleFor(c => c.LoginTime).NotEmpty();
        RuleFor(c => c.IsRevoked).NotEmpty();
        RuleFor(c => c.IsSuspicious).NotEmpty();
    }
}