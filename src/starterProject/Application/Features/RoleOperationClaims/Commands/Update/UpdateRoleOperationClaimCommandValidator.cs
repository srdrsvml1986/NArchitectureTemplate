using FluentValidation;

namespace Application.Features.RoleOperationClaims.Commands.Update;

public class UpdateRoleOperationClaimCommandValidator : AbstractValidator<UpdateRoleOperationClaimCommand>
{
    public UpdateRoleOperationClaimCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.OperationClaimId).NotEmpty();
        RuleFor(c => c.RoleId).NotEmpty();
    }
}