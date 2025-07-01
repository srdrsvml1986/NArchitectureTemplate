using System.Text.RegularExpressions;
using FluentValidation;

namespace Application.Features.Users.Commands.UpdateFromAuth;

public class UpdateUserFromAuthCommandValidator : AbstractValidator<UpdateUserPasswordFromAuthCommand>
{
    public UpdateUserFromAuthCommandValidator()
    {
        RuleFor(x => x.Password)
           .NotEmpty()
           .MinimumLength(4)
           .Matches(@"[A-Z]+").WithMessage("Şifre en az bir büyük harf içermelidir.")
           .Matches(@"[a-z]+").WithMessage("Şifre en az bir küçük harf içermelidir.")
           .Matches(@"[0-9]+").WithMessage("Şifre en az bir rakam içermelidir.");

        //RuleFor(c => c.NewPassword)
        //.NotEmpty()
        //.MinimumLength(5)
        //.Must(StrongPassword)
        //.WithMessage(
        //    "Şifreniz en az 5 karakter uzunluğunda olmalı ve en az bir büyük harf, bir küçük harf ve bir özel karakter içermelidir."
        //);

    }

    private bool StrongPassword(string arg)
    {
        Regex regex = new(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?+!@$%^&*-]).{5,}$");
        return regex.IsMatch(arg);
    }
}
