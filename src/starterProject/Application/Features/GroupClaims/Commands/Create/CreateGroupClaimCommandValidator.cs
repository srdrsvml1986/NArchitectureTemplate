using FluentValidation;

namespace Application.Features.GroupClaims.Commands.Create;

public class CreateGroupClaimCommandValidator : AbstractValidator<CreateGroupClaimCommand>
{
    public CreateGroupClaimCommandValidator()
    {
        RuleFor(c => c.ClaimId).NotEmpty();
        RuleFor(c => c.GroupId).NotEmpty();
    }
}