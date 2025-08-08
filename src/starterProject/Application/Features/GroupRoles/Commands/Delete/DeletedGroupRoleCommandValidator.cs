using FluentValidation;

namespace Application.Features.GroupRoles.Commands.Delete;

public class DeleteGroupRoleCommandValidator : AbstractValidator<DeleteGroupRoleCommand>
{
    public DeleteGroupRoleCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}