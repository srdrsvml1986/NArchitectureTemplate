namespace NArchitecture.Core.Application.Pipelines.Authorization;

public interface ICurrentUserAuthorizationService
{
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserGroupsAsync(Guid userId, CancellationToken cancellationToken = default);

}
