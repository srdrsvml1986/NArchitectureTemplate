using FluentValidation;

namespace Application.Features.GroupClaims.Commands.Delete;

public class DeleteGroupClaimCommandValidator : AbstractValidator<DeleteGroupClaimCommand>
{
    public DeleteGroupClaimCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}