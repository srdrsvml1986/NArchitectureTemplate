using FluentValidation;

namespace Application.Features.Users.Commands.AddUserClaims;

public class AddUserClaimsCommandValidator : AbstractValidator<AddUserClaimsCommand>
{
    public AddUserClaimsCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.ClaimIds).NotEmpty();
    }
}
