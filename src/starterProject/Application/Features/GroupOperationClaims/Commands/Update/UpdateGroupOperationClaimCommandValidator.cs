using FluentValidation;

namespace Application.Features.GroupOperationClaims.Commands.Update;

public class UpdateGroupOperationClaimCommandValidator : AbstractValidator<UpdateGroupOperationClaimCommand>
{
    public UpdateGroupOperationClaimCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.ClaimId).NotEmpty();
        RuleFor(c => c.GroupId).NotEmpty();
    }
}