using NArchitecture.Core.Security.Entities;

namespace NArchitecture.Core.Security.JWT;

public interface ITokenHelper<TUserId, TOperationClaimId, TRoleId, TRefreshTokenId>
{
    public AccessToken CreateToken(User<TUserId> user, IList<OperationClaim<TOperationClaimId>> operationClaims, IList<Role<TRoleId>> roles);

    public RefreshToken<TRefreshTokenId, TUserId> CreateRefreshToken(User<TUserId> user, string ipAddress);
}

