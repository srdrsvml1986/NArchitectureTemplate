using FluentValidation;

namespace Application.Features.DeviceTokens.Commands.Create;

public class CreateDeviceTokenCommandValidator : AbstractValidator<CreateDeviceTokenCommand>
{
    public CreateDeviceTokenCommandValidator()
    {
    }
}