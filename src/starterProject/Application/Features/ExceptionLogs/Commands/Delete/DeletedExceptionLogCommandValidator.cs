using FluentValidation;

namespace Application.Features.ExceptionLogs.Commands.Delete;

public class DeleteExceptionLogCommandValidator : AbstractValidator<DeleteExceptionLogCommand>
{
    public DeleteExceptionLogCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}