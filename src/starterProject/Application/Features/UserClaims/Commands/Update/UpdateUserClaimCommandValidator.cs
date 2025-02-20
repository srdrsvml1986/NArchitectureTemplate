using FluentValidation;

namespace Application.Features.UserClaims.Commands.Update;

public class UpdateUserClaimCommandValidator : AbstractValidator<UpdateUserClaimCommand>
{
    public UpdateUserClaimCommandValidator()
    {
        RuleFor(c => c.UserId).NotNull();
        RuleFor(c => c.OperationClaimId).GreaterThan(0);
    }
}
