using FluentValidation;

namespace Application.Features.RoleClaims.Commands.Delete;

public class DeleteRoleClaimCommandValidator : AbstractValidator<DeleteRoleClaimCommand>
{
    public DeleteRoleClaimCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}