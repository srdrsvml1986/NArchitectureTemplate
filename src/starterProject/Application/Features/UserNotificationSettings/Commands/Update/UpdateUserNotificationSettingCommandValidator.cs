using FluentValidation;

namespace Application.Features.UserNotificationSettings.Commands.Update;

public class UpdateUserNotificationSettingCommandValidator : AbstractValidator<UpdateUserNotificationSettingCommand>
{
    public UpdateUserNotificationSettingCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.NotificationType).NotEmpty();
        RuleFor(c => c.NotificationChannel).NotEmpty();
        RuleFor(c => c.IsEnabled).NotEmpty();
    }
}