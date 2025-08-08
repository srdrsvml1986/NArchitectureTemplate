using NArchitecture.Core.Persistence.Repositories;

namespace NArchitecture.Core.Security.Entities;

public class UserRole<TId, TUserId, TRoleId> : Entity<TId>
{
    public TUserId UserId { get; set; }
    public TRoleId RoleId { get; set; }

    public UserRole()
    {
        UserId = default!;
        RoleId = default!;
    }

    public UserRole(TUserId userId, TRoleId roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public UserRole(TId id, TUserId userId, TRoleId roleId)
        : base(id)
    {
        UserId = userId;
        RoleId = roleId;
    }
}