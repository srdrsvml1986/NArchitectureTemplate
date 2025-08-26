using NArchitectureTemplate.Core.Application.Responses;

namespace Application.Features.UserNotificationSettings.Commands.Delete;

public class DeletedUserNotificationSettingResponse : IResponse
{
    public Guid Id { get; set; }
}