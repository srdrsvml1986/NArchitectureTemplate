using NArchitecture.Core.Application.Responses;

namespace Application.Features.Users.Commands.ResetPassword;

public class ResetPasswordResponse : IResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
}
