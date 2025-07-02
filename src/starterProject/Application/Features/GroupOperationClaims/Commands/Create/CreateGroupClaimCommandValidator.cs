using FluentValidation;

namespace Application.Features.GroupOperationClaims.Commands.Create;

public class CreateGroupClaimCommandValidator : AbstractValidator<CreateGroupOperationClaimCommand>
{
    public CreateGroupClaimCommandValidator()
    {
        RuleFor(c => c.ClaimId).NotEmpty();
        RuleFor(c => c.GroupId).NotEmpty();
    }
}