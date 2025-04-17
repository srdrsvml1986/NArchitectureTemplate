using FluentValidation;

namespace Application.Features.SecurityClaims.Commands.Create;

public class CreateSecurityClaimCommandValidator : AbstractValidator<CreateSecurityClaimCommand>
{
    public CreateSecurityClaimCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MinimumLength(2);
    }
}
