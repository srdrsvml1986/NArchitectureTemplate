using Application.Services.Repositories;
using FluentValidation;

namespace Application.Features.Users.Commands.Create;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.FirstName)
    .NotEmpty().WithMessage("Ad alanı boş olamaz.")
    .MaximumLength(50).WithMessage("Ad 50 karakterden uzun olamaz.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad alanı boş olamaz.")
            .MaximumLength(50).WithMessage("Soyad 50 karakterden uzun olamaz.");

        RuleFor(x => x.Email)
.NotEmpty().WithMessage("Email adresi boş olamaz.")
.EmailAddress().WithMessage("Geçerli bir email adresi giriniz.")
.MustAsync(IsEmailUnique).WithMessage("Bu email adresi zaten kullanımda.");
        RuleFor(x => x.Password)
   .NotEmpty()
   .MinimumLength(4)
   .Matches(@"[A-Z]+").WithMessage("Şifre en az bir büyük harf içermelidir.")
   .Matches(@"[a-z]+").WithMessage("Şifre en az bir küçük harf içermelidir.")
   .Matches(@"[0-9]+").WithMessage("Şifre en az bir rakam içermelidir.");
        _userRepository = userRepository;
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
