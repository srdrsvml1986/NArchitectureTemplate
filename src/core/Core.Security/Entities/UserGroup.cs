using NArchitecture.Core.Persistence.Repositories;

namespace NArchitecture.Core.Security.Entities;

public class UserGroup<TId, TUserId, TGroupId> : Entity<TId>
{
    public TUserId UserId { get; set; }
    public TGroupId GroupId { get; set; }

    public UserGroup()
    {
        UserId = default!;
        GroupId = default!;
    }

    public UserGroup(TUserId userId, TGroupId groupId)
    {
        UserId = userId;
        GroupId = groupId;
    }

    public UserGroup(TId id, TUserId userId, TGroupId groupId)
        : base(id)
    {
        UserId = userId;
        GroupId = groupId;
    }
}