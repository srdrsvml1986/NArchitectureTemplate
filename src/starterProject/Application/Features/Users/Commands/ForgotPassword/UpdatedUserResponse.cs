using NArchitecture.Core.Application.Responses;

namespace Application.Features.Users.Commands.ForgotPassword;

public class ForgotPasswordResponse : IResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
}
