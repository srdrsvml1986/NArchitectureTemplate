using FluentValidation;

namespace Application.Features.GroupRoles.Commands.Update;

public class UpdateGroupRoleCommandValidator : AbstractValidator<UpdateGroupRoleCommand>
{
    public UpdateGroupRoleCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.GroupId).NotEmpty();
        RuleFor(c => c.RoleId).NotEmpty();
    }
}