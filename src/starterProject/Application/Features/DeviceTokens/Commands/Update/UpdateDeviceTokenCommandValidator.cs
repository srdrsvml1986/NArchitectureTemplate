using FluentValidation;

namespace Application.Features.DeviceTokens.Commands.Update;

public class UpdateDeviceTokenCommandValidator : AbstractValidator<UpdateDeviceTokenCommand>
{
    public UpdateDeviceTokenCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}