using FluentValidation;

namespace Application.Features.UserRoles.Commands.Create;

public class CreateUserRoleCommandValidator : AbstractValidator<CreateUserRoleCommand>
{
    public CreateUserRoleCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.RoleId).NotEmpty();
    }
}