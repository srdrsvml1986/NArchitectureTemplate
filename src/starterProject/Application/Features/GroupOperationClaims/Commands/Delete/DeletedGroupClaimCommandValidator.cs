using FluentValidation;

namespace Application.Features.GroupOperationClaims.Commands.Delete;

public class DeleteGroupOperationClaimCommandValidator : AbstractValidator<DeleteGroupOperationClaimCommand>
{
    public DeleteGroupOperationClaimCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}