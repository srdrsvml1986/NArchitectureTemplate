using FluentValidation;

namespace Application.Features.Logs.Commands.Update;

public class UpdateLogCommandValidator : AbstractValidator<UpdateLogCommand>
{
    public UpdateLogCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}