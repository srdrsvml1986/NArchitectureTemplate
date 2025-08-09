using Microsoft.IdentityModel.Tokens;
using NArchitectureTemplate.Core.Security.Entities;
using NArchitectureTemplate.Core.Security.Encryption;
using NArchitectureTemplate.Core.Security.Extensions;
using System.Collections.Immutable;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace NArchitectureTemplate.Core.Security.JWT;

public class JwtHelper<TUserId, TOperationClaimId, TRoleId, TGroupId, TRefreshTokenId> : ITokenHelper<TUserId, TOperationClaimId, TRoleId, TGroupId, TRefreshTokenId>
{
    private readonly TokenOptions _tokenOptions;

    public JwtHelper(TokenOptions tokenOptions)
    {
        _tokenOptions = tokenOptions;
    }

    public virtual AccessToken CreateToken(User<TUserId> user, IList<OperationClaim<TOperationClaimId>> operationClaims, IList<Role<TRoleId>> roles, IList<Group<TGroupId>> groups)
    {
        DateTime accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
        SecurityKey securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
        SigningCredentials signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
        JwtSecurityToken jwt = CreateJwtSecurityToken(
            _tokenOptions,
            user,
            signingCredentials,
            operationClaims,
            roles,
            groups,
            accessTokenExpiration
        );
        JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
        string? token = jwtSecurityTokenHandler.WriteToken(jwt);

        return new AccessToken() { Token = token, ExpirationDate = accessTokenExpiration };
    }

    public RefreshToken<TRefreshTokenId, TUserId> CreateRefreshToken(User<TUserId> user, string ipAddress)
    {
        return new RefreshToken<TRefreshTokenId, TUserId>()
        {
            UserId = user.Id,
            Token = randomRefreshToken(),
            ExpirationDate = DateTime.UtcNow.AddDays(_tokenOptions.RefreshTokenTTL),
            CreatedByIp = ipAddress
        };
    }

    public virtual JwtSecurityToken CreateJwtSecurityToken(
        TokenOptions tokenOptions,
        User<TUserId> user,
        SigningCredentials signingCredentials,
        IList<OperationClaim<TOperationClaimId>> operationClaims, 
        IList<Role<TRoleId>> roles,
        IList<Group<TGroupId>> groups,
        DateTime accessTokenExpiration
    )
    {
        return new JwtSecurityToken(
            tokenOptions.Issuer,
            tokenOptions.Audience,
            expires: accessTokenExpiration,
            notBefore: DateTime.Now,
            claims: SetClaims(user, operationClaims, roles,groups),
            signingCredentials: signingCredentials
        );
    }

    protected virtual IEnumerable<Claim> SetClaims(User<TUserId> user, IList<OperationClaim<TOperationClaimId>> operationClaims, IList<Role<TRoleId>> roles, IList<Group<TGroupId>> groups)
    {
        List<Claim> claims = [];
        claims.AddNameIdentifier(user!.Id!.ToString()!);
        claims.AddEmail(user.Email);
        claims.AddPermissions(operationClaims.Select(c => c.Name).ToArray());
        claims.AddRoles(roles.Select(r => r.Name).ToArray());
        claims.AddGroups(groups.Select(g => g.Name).ToArray());
        return claims.ToImmutableList();
    }

    private string randomRefreshToken()
    {
        byte[] numberByte = new byte[32];
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(numberByte);
        return Convert.ToBase64String(numberByte);
    }
}
