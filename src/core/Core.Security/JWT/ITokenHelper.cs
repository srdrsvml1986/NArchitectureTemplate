using NArchitectureTemplate.Core.Security.Entities;

namespace NArchitectureTemplate.Core.Security.JWT;

public interface ITokenHelper<TUserId, TOperationClaimId, TRoleId, TGroupId, TRefreshTokenId>
{
    public AccessToken CreateToken(User<TUserId> user, IList<OperationClaim<TOperationClaimId>> operationClaims, IList<Role<TRoleId>> roles, IList<Group<TGroupId>> groups);

    public RefreshToken<TRefreshTokenId, TUserId> CreateRefreshToken(User<TUserId> user, string ipAddress);
}

