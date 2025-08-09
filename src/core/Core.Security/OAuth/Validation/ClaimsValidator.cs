using NArchitectureTemplate.Core.Security.OAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NArchitectureTemplate.Core.Security.OAuth.Validation;
public class ClaimsValidator
{
    public static bool ValidateUserClaims(ExternalAuthUser user, ClaimsPrincipal claims)
    {
        if (!claims.HasClaim(c => c.Type == "sub" && c.Value == user.Id))
            return false;

        if (!claims.HasClaim(c => c.Type == "email" && c.Value == user.Email))
            return false;

        return true;
    }
}
