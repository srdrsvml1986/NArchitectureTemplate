using FluentValidation;

namespace Application.Features.UserGroups.Commands.Delete;

public class DeleteUserGroupCommandValidator : AbstractValidator<DeleteUserGroupCommand>
{
    public DeleteUserGroupCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}