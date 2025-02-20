using FluentValidation;

namespace Application.Features.UserClaims.Commands.Create;

public class CreateUserClaimCommandValidator : AbstractValidator<CreateUserClaimCommand>
{
    public CreateUserClaimCommandValidator()
    {
        RuleFor(c => c.UserId).NotNull();
        RuleFor(c => c.ClaimId).GreaterThan(0);
    }
}
