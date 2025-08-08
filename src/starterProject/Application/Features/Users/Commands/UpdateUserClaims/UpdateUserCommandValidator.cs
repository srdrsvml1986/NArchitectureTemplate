using FluentValidation;

namespace Application.Features.Users.Commands.UpdateUserClaims;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserClaimsCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.ClaimIds).NotEmpty();
    }
}