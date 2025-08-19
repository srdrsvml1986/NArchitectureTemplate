using FluentValidation;

namespace Application.Features.Logs.Commands.Delete;

public class DeleteLogCommandValidator : AbstractValidator<DeleteLogCommand>
{
    public DeleteLogCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}