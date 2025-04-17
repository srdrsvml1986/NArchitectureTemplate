using FluentValidation;

namespace Application.Features.UserSecurityClaims.Commands.Update;

public class UpdateUserSecurityClaimCommandValidator : AbstractValidator<UpdateUserSecurityClaimCommand>
{
    public UpdateUserSecurityClaimCommandValidator()
    {
        RuleFor(c => c.UserId).NotNull();
        RuleFor(c => c.OperationClaimId).GreaterThan(0);
    }
}
