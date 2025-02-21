using FluentValidation;

namespace Application.Features.Users.Commands.UpdatePhotoURL;

public class UpdateUserCommandValidator : AbstractValidator<UpdatePhotoUrlCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(c => c.PhotoURL).NotEmpty();
    }
}