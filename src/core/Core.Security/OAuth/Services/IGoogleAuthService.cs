using NArchitecture.Core.Security.OAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NArchitecture.Core.Security.OAuth.Services;
public interface IGoogleAuthService
{
    Task<OAuthResponse> AuthenticateAsync(string code);
    string GetAuthorizationUrl();
}
