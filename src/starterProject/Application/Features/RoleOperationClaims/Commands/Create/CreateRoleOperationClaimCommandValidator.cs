using FluentValidation;

namespace Application.Features.RoleOperationClaims.Commands.Create;

public class CreateRoleOperationClaimCommandValidator : AbstractValidator<CreateRoleOperationClaimCommand>
{
    public CreateRoleOperationClaimCommandValidator()
    {
        RuleFor(c => c.OperationClaimId).NotEmpty();
        RuleFor(c => c.RoleId).NotEmpty();
    }
}