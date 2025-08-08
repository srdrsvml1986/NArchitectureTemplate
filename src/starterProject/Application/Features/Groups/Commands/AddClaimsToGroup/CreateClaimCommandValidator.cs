using FluentValidation;

namespace Application.Features.Groups.Commands.AddClaimsToGroup;

public class CreateGroupCommandValidator : AbstractValidator<AddClaimsToGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(c => c.ClaimIds).NotEmpty();
        RuleFor(c => c.GroupId).NotEmpty();
    }
}