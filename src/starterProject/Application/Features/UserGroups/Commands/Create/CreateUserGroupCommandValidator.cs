using FluentValidation;

namespace Application.Features.UserGroups.Commands.Create;

public class CreateUserGroupCommandValidator : AbstractValidator<CreateUserGroupCommand>
{
    public CreateUserGroupCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.GroupId).NotEmpty();
    }
}