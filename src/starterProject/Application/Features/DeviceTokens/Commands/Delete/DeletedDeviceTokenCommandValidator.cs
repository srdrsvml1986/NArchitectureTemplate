using FluentValidation;

namespace Application.Features.DeviceTokens.Commands.Delete;

public class DeleteDeviceTokenCommandValidator : AbstractValidator<DeleteDeviceTokenCommand>
{
    public DeleteDeviceTokenCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}