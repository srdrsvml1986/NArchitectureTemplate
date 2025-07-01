using FluentValidation;

namespace Application.Features.Users.Commands.AddUserRoles;

public class AddUserRolesCommandValidator : AbstractValidator<AddUserRolesCommand>
{
    public AddUserRolesCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.RoleIds).NotEmpty();
    }
}
