using FluentValidation;

namespace Application.Features.GroupClaims.Commands.Update;

public class UpdateGroupClaimCommandValidator : AbstractValidator<UpdateGroupClaimCommand>
{
    public UpdateGroupClaimCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.ClaimId).NotEmpty();
        RuleFor(c => c.GroupId).NotEmpty();
    }
}