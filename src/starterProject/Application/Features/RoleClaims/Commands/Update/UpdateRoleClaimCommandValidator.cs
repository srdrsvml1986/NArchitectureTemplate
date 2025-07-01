using FluentValidation;

namespace Application.Features.RoleClaims.Commands.Update;

public class UpdateRoleClaimCommandValidator : AbstractValidator<UpdateRoleClaimCommand>
{
    public UpdateRoleClaimCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.ClaimId).NotEmpty();
        RuleFor(c => c.RoleId).NotEmpty();
    }
}