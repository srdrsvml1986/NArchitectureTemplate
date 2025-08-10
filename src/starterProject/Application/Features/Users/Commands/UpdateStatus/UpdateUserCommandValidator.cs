
using FluentValidation;

namespace Application.Features.Users.Commands.UpdateStatus;

public class UpdateUserStatusCommandValidator : AbstractValidator<UpdateUserStatusCommand>
{
    public UpdateUserStatusCommandValidator()
    {
        RuleFor(c => c.Status).NotEmpty();
    }
}
