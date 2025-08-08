using NArchitecture.Core.Persistence.Repositories;

namespace NArchitecture.Core.Security.Entities;

public class GroupRole<TId, TGroupId, TRoleId> : Entity<TId>
{
    public TGroupId GroupId { get; set; }
    public TRoleId RoleId { get; set; }

    public GroupRole()
    {
        GroupId = default!;
        RoleId = default!;
    }

    public GroupRole(TGroupId groupId, TRoleId roleId)
    {
        GroupId = groupId;
        RoleId = roleId;
    }

    public GroupRole(TId id, TGroupId groupId, TRoleId roleId)
        : base(id)
    {
        GroupId = groupId;
        RoleId = roleId;
    }
}
