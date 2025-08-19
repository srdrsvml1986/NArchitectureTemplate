using FluentValidation;

namespace Application.Features.Logs.Commands.Create;

public class CreateLogCommandValidator : AbstractValidator<CreateLogCommand>
{
    public CreateLogCommandValidator()
    {
    }
}