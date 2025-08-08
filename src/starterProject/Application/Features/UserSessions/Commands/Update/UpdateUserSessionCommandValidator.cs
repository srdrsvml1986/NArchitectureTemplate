using FluentValidation;

namespace Application.Features.UserSessions.Commands.Update;

public class UpdateUserSessionCommandValidator : AbstractValidator<UpdateUserSessionCommand>
{
    public UpdateUserSessionCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.IpAddress).NotEmpty();
        RuleFor(c => c.UserAgent).NotEmpty();
        RuleFor(c => c.LoginTime).NotEmpty();
        RuleFor(c => c.IsRevoked).NotEmpty();
        RuleFor(c => c.IsSuspicious).NotEmpty();
    }
}