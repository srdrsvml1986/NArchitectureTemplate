using FluentValidation;

namespace Application.Features.RoleOperationClaims.Commands.Delete;

public class DeleteRoleOperationClaimCommandValidator : AbstractValidator<DeleteRoleOperationClaimCommand>
{
    public DeleteRoleOperationClaimCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}