using FluentValidation;

namespace Application.Features.SecurityClaims.Commands.Update;

public class UpdateSecurityClaimCommandValidator : AbstractValidator<UpdateSecurityClaimCommand>
{
    public UpdateSecurityClaimCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MinimumLength(2);
    }
}
