using FluentValidation;

namespace Application.Features.UserSessions.Commands.Delete;

public class DeleteUserSessionCommandValidator : AbstractValidator<DeleteUserSessionCommand>
{
    public DeleteUserSessionCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}