using FluentValidation;

namespace Application.Features.UserSecurityClaims.Commands.Create;

public class CreateUserSecurityClaimCommandValidator : AbstractValidator<CreateUserSecurityClaimCommand>
{
    public CreateUserSecurityClaimCommandValidator()
    {
        RuleFor(c => c.UserId).NotNull();
        RuleFor(c => c.ClaimId).GreaterThan(0);
    }
}
