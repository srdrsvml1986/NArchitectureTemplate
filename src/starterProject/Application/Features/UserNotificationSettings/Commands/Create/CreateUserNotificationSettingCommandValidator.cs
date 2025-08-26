using FluentValidation;

namespace Application.Features.UserNotificationSettings.Commands.Create;

public class CreateUserNotificationSettingCommandValidator : AbstractValidator<CreateUserNotificationSettingCommand>
{
    public CreateUserNotificationSettingCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.NotificationType).NotEmpty();
        RuleFor(c => c.NotificationChannel).NotEmpty();
        RuleFor(c => c.IsEnabled).NotEmpty();
    }
}