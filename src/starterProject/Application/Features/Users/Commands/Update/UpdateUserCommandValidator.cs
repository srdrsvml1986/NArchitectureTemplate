using Application.Services.Repositories;
using FluentValidation;

namespace Application.Features.Users.Commands.Update;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Email)
    .NotEmpty().WithMessage("Email adresi boş olamaz.")
    .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.")
    .MustAsync(IsEmailUnique).WithMessage("Bu email adresi zaten kullanımda.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad alanı boş olamaz.")
            .MaximumLength(50).WithMessage("Ad 50 karakterden uzun olamaz.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad alanı boş olamaz.")
            .MaximumLength(50).WithMessage("Soyad 50 karakterden uzun olamaz.");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^(\+[0-9]{1,3}|0)[0-9]{10}$")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }

    private async Task<bool> IsEmailUnique(string email, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetAsync(
            predicate: u => u.Email == email,
            enableTracking: false,
            cancellationToken: cancellationToken
        );

        return existingUser == null;
    }
}
