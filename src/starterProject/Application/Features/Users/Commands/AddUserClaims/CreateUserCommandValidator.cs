using FluentValidation;

namespace Application.Features.Users.Commands.AddUserClaims;

public class CreateUserCommandValidator : AbstractValidator<AddUserClaimsCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.ClaimIds).NotEmpty();
    }
}
