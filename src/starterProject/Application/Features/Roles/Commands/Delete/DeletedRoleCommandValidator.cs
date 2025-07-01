using FluentValidation;

namespace Application.Features.Roles.Commands.Delete;

public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}