using FluentValidation;

namespace Application.Features.RoleClaims.Commands.Create;

public class CreateRoleClaimCommandValidator : AbstractValidator<CreateRoleClaimCommand>
{
    public CreateRoleClaimCommandValidator()
    {
        RuleFor(c => c.ClaimId).NotEmpty();
        RuleFor(c => c.RoleId).NotEmpty();
    }
}