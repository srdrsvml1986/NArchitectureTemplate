using FluentValidation;

namespace Application.Features.GroupRoles.Commands.Create;

public class CreateGroupRoleCommandValidator : AbstractValidator<CreateGroupRoleCommand>
{
    public CreateGroupRoleCommandValidator()
    {
        RuleFor(c => c.GroupId).NotEmpty();
        RuleFor(c => c.RoleId).NotEmpty();
    }
}