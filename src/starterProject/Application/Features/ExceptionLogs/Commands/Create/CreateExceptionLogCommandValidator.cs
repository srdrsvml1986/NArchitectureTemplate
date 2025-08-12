using FluentValidation;

namespace Application.Features.ExceptionLogs.Commands.Create;

public class CreateExceptionLogCommandValidator : AbstractValidator<CreateExceptionLogCommand>
{
    public CreateExceptionLogCommandValidator()
    {
    }
}