using FluentValidation;

namespace Application.Features.Groups.Commands.UpdateClaimsInGroup;

public class UpdateGroupCommandValidator : AbstractValidator<UpdateClaimsInGroupCommand>
{
    public UpdateGroupCommandValidator()
    {
        RuleFor(c => c.GroupId).NotEmpty();
        RuleFor(c => c.ClaimIds).NotEmpty();
    }
}