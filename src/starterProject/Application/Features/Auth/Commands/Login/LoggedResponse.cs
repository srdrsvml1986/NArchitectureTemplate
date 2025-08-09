using NArchitectureTemplate.Core.Application.Responses;
using NArchitectureTemplate.Core.Security.Enums;
using NArchitectureTemplate.Core.Security.JWT;

namespace Application.Features.Auth.Commands.Login;

public class LoggedResponse : IResponse
{
    public AccessToken? AccessToken { get; set; }
    public Domain.Entities.RefreshToken? RefreshToken { get; set; }
    public AuthenticatorType? RequiredAuthenticatorType { get; set; }

    public LoggedHttpResponse ToHttpResponse()
    {
        return new() { AccessToken = AccessToken, RefreshToken = RefreshToken?.Token, RequiredAuthenticatorType = RequiredAuthenticatorType };
    }

    public class LoggedHttpResponse
    {
        public AccessToken? AccessToken { get; set; }
        public String? RefreshToken { get; set; }
        public AuthenticatorType? RequiredAuthenticatorType { get; set; }
    }
}
