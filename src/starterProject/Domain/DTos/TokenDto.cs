using Domain.Entities;
using NArchitecture.Core.Security.JWT;

namespace Domain.DTos;
public class TokenDto
{
    public AccessToken AccessToken { get; set; }
    public RefreshToken RefreshToken { get; set; }
}
