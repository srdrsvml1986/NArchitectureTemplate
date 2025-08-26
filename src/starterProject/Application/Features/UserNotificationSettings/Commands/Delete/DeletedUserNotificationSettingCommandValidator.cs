using FluentValidation;

namespace Application.Features.UserNotificationSettings.Commands.Delete;

public class DeleteUserNotificationSettingCommandValidator : AbstractValidator<DeleteUserNotificationSettingCommand>
{
    public DeleteUserNotificationSettingCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}