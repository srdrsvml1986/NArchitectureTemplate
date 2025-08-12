using FluentValidation;

namespace Application.Features.ExceptionLogs.Commands.Update;

public class UpdateExceptionLogCommandValidator : AbstractValidator<UpdateExceptionLogCommand>
{
    public UpdateExceptionLogCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}